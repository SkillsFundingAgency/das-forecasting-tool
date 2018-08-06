using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.UnitTests.Commitments.StoreCommitmentHandler
{
    public class WhenIStoreCommitments
    {
        private AutoMoq.AutoMoqer _moqer;
        private Mock<IEmployerCommitmentRepository> _employerCommitmentRepostiory;
        private Mock<ILog> _logger;
        private Application.Commitments.Handlers.StoreCommitmentHandler _handler;
        private Mock<IPaymentMapper> _paymentMapper;
        private PaymentCreatedMessage _message;
        private CommitmentModel _newCommitmentModel;
        private CommitmentModel _dbCommitmentModel;
        private const long ExpectedEmployerAccountId = 554433;
        private const long ExpectedApprenticeshipId = 552244;

        [SetUp]
        public void Arrange()
        {
            _moqer = new AutoMoqer();
            _message = new PaymentCreatedMessage
            {
                EmployerAccountId = ExpectedEmployerAccountId,
                ApprenticeshipId = ExpectedApprenticeshipId,
                EarningDetails = new EarningDetails()
            };
            _newCommitmentModel = new CommitmentModel
            {
                EmployerAccountId = ExpectedEmployerAccountId,
                ActualEndDate = DateTime.Now.AddMonths(1),
                Id = 3322,
                CompletionAmount = 100
            };
            _dbCommitmentModel = new CommitmentModel
            {
                EmployerAccountId = ExpectedEmployerAccountId,
                ActualEndDate = DateTime.Now.AddMonths(1),
                Id = 3322,
                CompletionAmount = 50
            };

            _moqer.GetMock<ICommitmentValidator>().Setup(x => x.IsValid(It.IsAny<CommitmentModel>())).Returns(true);
            _employerCommitmentRepostiory = _moqer.GetMock<IEmployerCommitmentRepository>();
            _employerCommitmentRepostiory.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new EmployerCommitment(_dbCommitmentModel, _moqer.GetMock<ICommitmentValidator>().Object));
            _logger = _moqer.GetMock<ILog>();

            _paymentMapper = _moqer.GetMock<IPaymentMapper>();
            _paymentMapper.Setup(x =>
                    x.MapToCommitment(It.Is<PaymentCreatedMessage>(c =>
                        c.EmployerAccountId.Equals(ExpectedEmployerAccountId))))
                .Returns(_newCommitmentModel);

            _handler = _moqer.Resolve<Application.Commitments.Handlers.StoreCommitmentHandler>();
        }

        [Test]
        public void Then_An_Invalid_Operation_Exception_Is_Thrown_If_The_Message_Is_Not_Valid()
        {
            //Arrange
            var message = new PaymentCreatedMessage();

            //Act
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(message));
        }

        [Test]
        public async Task Then_The_Commitment_Is_Mapped_If_The_Message_Is_Valid()
        {
            //Act
            await _handler.Handle(_message);

            //Assert
            _paymentMapper.Verify(x => x.MapToCommitment(_message), Times.Once);
        }

        [Test]
        public async Task Then_The_Commitment_Is_Stored_By_The_Repository()
        {
            //Act
            await _handler.Handle(_message);

            //Assert
            _employerCommitmentRepostiory.Verify(x =>
                x.Store(It.Is<EmployerCommitment>(c => c.EmployerAccountId.Equals(ExpectedEmployerAccountId))));
        }

        [Test]
        public async Task Then_The_Commitment_Is_Not_Stored_If_Commitment_Details_Have_Not_Changed()
        {
            _newCommitmentModel.CompletionAmount = _dbCommitmentModel.CompletionAmount;
            await _handler.Handle(_message);

            _employerCommitmentRepostiory.Verify(x => x.Store(It.IsAny<EmployerCommitment>()),Times.Never);
        }

    }
}