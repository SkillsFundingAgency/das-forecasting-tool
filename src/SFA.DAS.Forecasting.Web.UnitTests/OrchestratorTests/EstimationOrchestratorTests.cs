using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    public class EstimationOrchestratorTests
    {
        private EstimationOrchestrator _orchestrator;
        private Mock<IHashingService> _hashingService;
        private Mock<ICurrentBalanceRepository> _currentBalanceRepository;
        private Mock<IAccountEstimationRepository> _accountEstimationRepository;
        private Mock<IAccountEstimationProjectionRepository> _accountEstimationProjectionRepository;
        private Mock<IAccountEstimationProjection> _accountEstimationProjection;
        private const long ExpectedAccountId = 654311;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(ExpectedAccountId);


            var currentBalance = new Mock<CurrentBalance>();
            currentBalance.Setup(x => x.RefreshBalance()).ReturnsAsync(true);

            _currentBalanceRepository = new Mock<ICurrentBalanceRepository>();
            _currentBalanceRepository.Setup(x => x.Get(ExpectedAccountId))
                .ReturnsAsync(currentBalance.Object);

            _accountEstimationRepository = new Mock<IAccountEstimationRepository>();

            _accountEstimationProjection = new Mock<IAccountEstimationProjection>();
            _accountEstimationProjection.Setup(x => x.Projections)
                .Returns((new List<AccountEstimationProjectionModel>()).AsReadOnly);

            _accountEstimationProjectionRepository = new Mock<IAccountEstimationProjectionRepository>();
            _accountEstimationProjectionRepository.Setup(x => x.Get(It.IsAny<AccountEstimation>()))
                .ReturnsAsync(_accountEstimationProjection.Object);

            _orchestrator = new EstimationOrchestrator(_accountEstimationProjectionRepository.Object,
                _accountEstimationRepository.Object, _hashingService.Object,
                _currentBalanceRepository.Object,
                Mock.Of<IApprenticeshipCourseDataService>());
        }

        [TestCase(100, 100, 100, 100)]
        [TestCase(150, 100, 100, 100)]
        [TestCase(100, 160, 100, 100)]
        [TestCase(100, 100, 105.55, 100)]
        [TestCase(100, 100, 105.55, 200)]
        public async Task Then_The_Cost_Takes_Into_Account_The_Actual_And_Estimated_Payments(
            decimal actualTotalCostOfTraining,
            decimal actualCommittedCompletionPayments,
            decimal transferOutTotalCostOfTraining,
            decimal transferOutCompletionPayment)
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) DateTime.Now.AddYears(1).Year,
                    Month = (short) DateTime.Now.Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        TransferOutCostOfTraining =  actualTotalCostOfTraining,
                        TransferOutCompletionPayments = actualCommittedCompletionPayments,
                    },
                    ModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        TransferOutCostOfTraining =  transferOutTotalCostOfTraining,
                        TransferOutCompletionPayments = transferOutCompletionPayment,
                    }

                }
            };
            _accountEstimationProjection.Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.AreEqual(actualTotalCostOfTraining + actualCommittedCompletionPayments, actual.TransferAllowances.First().ActualCost);
            Assert.AreEqual(transferOutTotalCostOfTraining + transferOutCompletionPayment, actual.TransferAllowances.First().EstimatedCost);
        }

        [Test]
        public async Task Then_The_Levy_Out_Costs_Are_Not_factored_Into_The_Acutal_Costs()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) DateTime.Now.AddYears(1).Year,
                    Month = (short) DateTime.Now.Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 100m,
                        LevyCompletionPayments = 100m,
                        TransferOutCostOfTraining =  50m,
                        TransferOutCompletionPayments = 50m
                    }
                }
            };
            _accountEstimationProjection.Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.AreEqual(100m,actual.TransferAllowances.First().ActualCost);

        }

        [Test]
        public async Task Then_The_IsLessThanCost_Flag_Uses_Actual_And_Estimated_Values()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) DateTime.Now.AddYears(1).Year,
                    Month = (short) DateTime.Now.Month,
                    ModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 50},
                    FutureFunds = 100
                }
            };
            _accountEstimationProjection.Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsTrue(actual.TransferAllowances.First().IsLessThanCost);
        }
    }
}