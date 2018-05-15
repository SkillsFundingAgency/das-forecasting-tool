using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Balance;
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
                _currentBalanceRepository.Object);
        }

        [TestCase(100,100,100,300)]
        [TestCase(150, 100, 100, 350)]
        [TestCase(100, 160, 100, 360)]
        [TestCase(100, 100, 105.55, 305.55)]
        public async Task Then_The_Cost_Takes_Into_Account_The_CompletionPayments_And_TransferOutCompletionPayments_And_CostOfTraining(
            decimal totalCostOfTraining, decimal completionPayments, decimal transferOutPayment, decimal expectedTotalCost)
        {
            
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) DateTime.Now.AddYears(1).Year,
                    Month = (short) DateTime.Now.Month,
                    TotalCostOfTraining = totalCostOfTraining,
                    CompletionPayments = completionPayments,
                    TransferOutCompletionPayments = transferOutPayment
                }
            };
            _accountEstimationProjection.Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.AreEqual(expectedTotalCost, actual.TransferAllowances.First().Cost);
        }
    }
}