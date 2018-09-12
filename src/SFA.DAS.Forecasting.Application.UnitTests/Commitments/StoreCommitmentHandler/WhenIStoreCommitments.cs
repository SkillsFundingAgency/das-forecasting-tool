using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.Commitments.StoreCommitmentHandler
{
    public class WhenIStoreCommitments
    {
        private Mock<IEmployerCommitmentRepository> _employerCommitmentRepostiory;
        private Mock<ILog> _logger;
        private Application.Commitments.Handlers.StoreCommitmentHandler _handler;
        private Mock<IPaymentMapper> _paymentMapper;
        private PaymentCreatedMessage _message;
        private Mock<IQueueService> _queueServiceMock;
        private CommitmentModel _commitmentModel;
        private const long ExpectedEmployerAccountId = 554433;
        private const long ExpectedApprenticeshipId = 552244;
        private string _allowProjectionQueueName = "forecasting-payment-allow-projection";

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
                ActualEndDate = DateTime.Now.AddMonths(1),
                Id = 3322,
                CompletionAmount = 100
            };

            _employerCommitmentRepostiory = new Mock<IEmployerCommitmentRepository>();

            _logger = new Mock<ILog>();

            _paymentMapper = new Mock<IPaymentMapper>();
            _paymentMapper.Setup(x =>
                    x.MapToCommitment(It.Is<PaymentCreatedMessage>(c =>
                        c.EmployerAccountId.Equals(ExpectedEmployerAccountId))))
                .Returns(_commitmentModel);
            

            _queueServiceMock = new Mock<IQueueService>();

            _handler = new Application.Commitments.Handlers.StoreCommitmentHandler(
                _employerCommitmentRepostiory.Object,
                _logger.Object, 
                _paymentMapper.Object, 
                new Mock<ITelemetry>().Object, 
                new Apprenticeship.Mapping.ApprenticeshipMapping(), _queueServiceMock.Object);
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
        public async Task Then_The_Commitment_Is_Mapped_If_The_Message_Is_Valid()
        {
            //Act
            await _handler.Handle(_message, _allowProjectionQueueName);

            //Assert
            _paymentMapper.Verify(x => x.MapToCommitment(_message), Times.Once);
        }

        [Test]
        public async Task Then_The_Commitment_Is_Upserted_In_The_Repository()
        {
            //Act
            await _handler.Handle(_message, _allowProjectionQueueName);

            //Assert
            _employerCommitmentRepostiory.Verify(x =>
                x.Upsert(It.Is<CommitmentModel>(c => c.EmployerAccountId.Equals(ExpectedEmployerAccountId))));
        }

        [Test]
        public async Task Then_A_PaymentCreatedMessage_Is_Queued_On_AllowProjection_Queue()
        {
            //Act
            await _handler.Handle(_message, _allowProjectionQueueName);

            //Assert
            _queueServiceMock.Verify(v => v.SendMessageWithVisibilityDelay(It.Is<PaymentCreatedMessage>(m => m == _message), It.Is<string>(s => s ==_allowProjectionQueueName)));

        }

    }
}