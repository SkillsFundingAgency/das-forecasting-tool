using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Application.UnitTests.Commitments.StoreCommitmentHandler;

public class WhenIStoreCommitments
{
    private Mock<IEmployerCommitmentRepository> _employerCommitmentRepository;
    private Mock<ILogger<Application.Commitments.Handlers.StoreCommitmentHandler>> _logger;
    private Application.Commitments.Handlers.StoreCommitmentHandler _handler;
    private Mock<IPaymentMapper> _paymentMapper;
    private PaymentCreatedMessage _message;
    private Mock<IQueueService> _queueServiceMock;
    private CommitmentModel _commitmentModel;
    private EmployerCommitment _employerCommitment;
    private const long ExpectedEmployerAccountId = 554433;
    private const long ExpectedApprenticeshipId = 552244;
    private readonly string _allowProjectionQueueName = "forecasting-payment-allow-projection";

    [SetUp]
    public void Arrange()
    {
        _message = new PaymentCreatedMessage
        {
            EmployerAccountId = ExpectedEmployerAccountId,
            ApprenticeshipId = ExpectedApprenticeshipId,
            EarningDetails = new EarningDetails()
        };
        _commitmentModel = new CommitmentModel
        {
            EmployerAccountId = ExpectedEmployerAccountId,
            CompletionAmount = 100
        };
            
        _employerCommitment = new EmployerCommitment(_commitmentModel);
            
        _employerCommitmentRepository = new Mock<IEmployerCommitmentRepository>();

        _logger = new Mock<ILogger<Application.Commitments.Handlers.StoreCommitmentHandler>>();

        _paymentMapper = new Mock<IPaymentMapper>();
        _paymentMapper.Setup(x =>
                x.MapToCommitment(It.Is<PaymentCreatedMessage>(c =>
                    c.EmployerAccountId.Equals(ExpectedEmployerAccountId))))
            .Returns(_commitmentModel);
        _employerCommitmentRepository.Setup(x => x.Get(ExpectedEmployerAccountId, ExpectedApprenticeshipId))
            .ReturnsAsync(_employerCommitment);

        _queueServiceMock = new Mock<IQueueService>();
        _handler = new Application.Commitments.Handlers.StoreCommitmentHandler(
            _employerCommitmentRepository.Object,
            _logger.Object, 
            _paymentMapper.Object, _queueServiceMock.Object);
    }

    [Test]
    public void Then_An_Invalid_Operation_Exception_Is_Thrown_If_The_Message_Is_Not_Valid()
    {
        //Arrange
        var message = new PaymentCreatedMessage();

        //Act
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(message,_allowProjectionQueueName));
    }

    [Test]
    public async Task Then_The_Commtiemtn_Is_Retrieved_From_The_Repository()
    {
        //Act
        await _handler.Handle(_message, _allowProjectionQueueName);

        //Assert
        _employerCommitmentRepository.Verify(x => x.Get(ExpectedEmployerAccountId, ExpectedApprenticeshipId));
    }

    [Test]
    public async Task Then_The_Commitment_Is_Mapped_If_The_Message_Is_Valid()
    {
        //Act
        await _handler.Handle(_message, _allowProjectionQueueName);

        //Assert
        _paymentMapper.Verify(x => x.MapToCommitment(_message), Times.Once);
    }

    [Test]
    public async Task Then_The_Commitment_Is_Stored_In_The_Repository()
    {
        //Act
        await _handler.Handle(_message, _allowProjectionQueueName);

        //Assert
        _queueServiceMock.Verify(v => v.SendMessageWithVisibilityDelay(It.Is<PaymentCreatedMessage>(m => m == _message), It.Is<string>(s => s ==_allowProjectionQueueName)));
        _employerCommitmentRepository.Verify(x => x.Store(It.IsAny<EmployerCommitment>()), Times.Once());
    }

    [Test]
    public async Task Then_The_Commitment_Is_Not_Stored_If_The_Registration_Check_Fails()
    {
        //Arrange
        _commitmentModel = new CommitmentModel
        {
            ApprenticeshipId = ExpectedApprenticeshipId,
            EmployerAccountId = ExpectedEmployerAccountId,
            ActualEndDate = null,
            Id = 3322,
            CompletionAmount = 100
        };
        _paymentMapper.Setup(x =>
                x.MapToCommitment(It.Is<PaymentCreatedMessage>(c =>
                    c.EmployerAccountId.Equals(ExpectedEmployerAccountId))))
            .Returns(_commitmentModel);
        _employerCommitment = new EmployerCommitment(_commitmentModel);
        _employerCommitmentRepository.Setup(x => x.Get(ExpectedEmployerAccountId, ExpectedApprenticeshipId))
            .ReturnsAsync(_employerCommitment);
        _handler = new Application.Commitments.Handlers.StoreCommitmentHandler(_employerCommitmentRepository.Object, _logger.Object, _paymentMapper.Object,  _queueServiceMock.Object);

        //Act
        await _handler.Handle(_message, _allowProjectionQueueName);

        //Assert
        _queueServiceMock.Verify(v => v.SendMessageWithVisibilityDelay(It.Is<PaymentCreatedMessage>(m => m == _message), It.Is<string>(s => s == _allowProjectionQueueName)), Times.Never);
        _employerCommitmentRepository.Verify(x => x.Store(It.IsAny<EmployerCommitment>()), Times.Never());
    }

}