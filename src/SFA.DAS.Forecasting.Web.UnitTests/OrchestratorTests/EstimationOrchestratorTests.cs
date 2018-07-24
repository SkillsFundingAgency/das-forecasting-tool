using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMoq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.HashingService;
using static SFA.DAS.Forecasting.Models.Estimation.AccountEstimationProjectionModel;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests
{
    public class EstimationOrchestratorTests
    {
        private EstimationOrchestrator _orchestrator;
        private AutoMoqer _mocker;
        private IAccountEstimationProjection _accountEstimationProjection;
        private const long ExpectedAccountId = 654311;

        [SetUp]
        public void Arrange()
        {
            _mocker = new AutoMoqer();

            _mocker.GetMock<IHashingService>()
                .Setup(x => x.DecodeValue(It.IsAny<string>()))
                .Returns(ExpectedAccountId);

            _mocker.GetMock<CurrentBalance>()
                .Setup(x => x.RefreshBalance(It.IsAny<bool>())).ReturnsAsync(true);

            _mocker.GetMock<ICurrentBalanceRepository>()
                .Setup(x => x.Get(ExpectedAccountId))
                .ReturnsAsync(_mocker.Resolve<CurrentBalance>());

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns((new List<AccountEstimationProjectionModel>()).AsReadOnly);
            _accountEstimationProjection = _mocker.Resolve<IAccountEstimationProjection>();

            _mocker.GetMock<IAccountEstimationProjectionRepository>()
                .Setup(x => x.Get(It.IsAny<AccountEstimation>()))
                .ReturnsAsync(_accountEstimationProjection);

            _orchestrator = _mocker.Resolve<EstimationOrchestrator>();
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

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
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
            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
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
                    FutureFunds = -100
                }
            };
            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsTrue(actual.TransferAllowances.First().IsLessThanCost);
        }

        [Test]
        public async Task AccountFunds_balance_should_include_projection_future_funds()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var estimationProjectionList = fixture.CreateMany<AccountEstimationProjectionModel>(48).ToList();
            var date = DateTime.Today;
            decimal projectedFutureFunds = 0;
            foreach (var estimationProjection in estimationProjectionList)
            {
                projectedFutureFunds += 600;
                estimationProjection.ProjectedFutureFunds = projectedFutureFunds;
                estimationProjection.ActualCosts =
                    new Cost
                    {
                        TransferOutCostOfTraining =  0,
                        TransferOutCompletionPayments = 0,
                        TransferInCostOfTraining = 0,
                        TransferInCompletionPayments = 0,
                        
                    };
                estimationProjection.ModelledCosts =
                    new Cost
                    {
                        LevyCostOfTraining = 0,
                        LevyCompletionPayments = 0,
                        TransferOutCostOfTraining = 0,
                        TransferOutCompletionPayments = 0
                    };

                estimationProjection.Month = (short)date.Month;
                estimationProjection.Year = (short)date.Year;
                date = date.AddMonths(1);
            }

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(estimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            actual.AccountFunds.OpeningBalance.Should().Be(600);

            var first = actual.AccountFunds.Records.First();
            var second = actual.AccountFunds.Records.Skip(1).First();

            first.ActualCost.Should().Be(0);
            first.EstimatedCost.Should().Be(0);
            first.Balance.Should().Be(600);

            second.ActualCost.Should().Be(0);
            second.EstimatedCost.Should().Be(0);
            second.Balance.Should().Be(1200);
        }

        [Test]
        public async Task AccountFunds_the_estimated_cost_for_each_months_should_be_removed_from_balance()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var estimationProjectionList = fixture.CreateMany<AccountEstimationProjectionModel>(48).ToList();
            var date = DateTime.Today;
            decimal projectedFutureFunds = 0;
            foreach (var estimationProjection in estimationProjectionList)
            {
                projectedFutureFunds += 600;
                estimationProjection.ProjectedFutureFunds = projectedFutureFunds;
                estimationProjection.ActualCosts =
                    new Cost
                    {
                        LevyCostOfTraining = 100,
                        LevyCompletionPayments = 0,
                        TransferOutCostOfTraining = 0,
                        TransferOutCompletionPayments = 0,
                        TransferInCostOfTraining = 0,
                        TransferInCompletionPayments = 0,

                    };
                estimationProjection.ModelledCosts =
                    new Cost
                    {
                        LevyCostOfTraining = 200,
                        LevyCompletionPayments = 0,
                        TransferOutCostOfTraining = 0,
                        TransferOutCompletionPayments = 0
                    };

                estimationProjection.Month = (short)date.Month;
                estimationProjection.Year = (short)date.Year;
                date = date.AddMonths(1);
            }

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(estimationProjectionList.AsReadOnly);

            // Act
            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            // Assert
            actual.AccountFunds.OpeningBalance.Should().Be(600);

            var first = actual.AccountFunds.Records.First();
            var second = actual.AccountFunds.Records.Skip(1).First();

            first.ActualCost.Should().Be(100);
            first.EstimatedCost.Should().Be(200);
            first.Balance.Should().Be(400);

            second.ActualCost.Should().Be(100);
            second.EstimatedCost.Should().Be(200);
            second.Balance.Should()
                .Be(800, 
                    because: "Month should have a balance of 1200 from the projection minus 2 months of estimation cost (2* 200)");
        }

        [Test]
        public async Task AccountFunds_CI_654_For_Sending_Employer()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var estimationProjectionList = fixture.CreateMany<AccountEstimationProjectionModel>(48).ToList();
            var date = DateTime.Today;
            decimal projectedFutureFunds = 8000 + 1300; // 1300 is the first month actual cost
            foreach (var estimationProjection in estimationProjectionList)
            {
                estimationProjection.ActualCosts =
                    new Cost
                    {
                        LevyCostOfTraining = 800,
                        LevyCompletionPayments = date.Month == DateTime.Today.Month +1 ? 1000 : 0,
                        TransferOutCostOfTraining = 500,
                        TransferOutCompletionPayments = date.Month == DateTime.Today.Month + 1 ? 800 : 0,
                        TransferInCostOfTraining = 0,
                        TransferInCompletionPayments = 0,
                    };
                estimationProjection.ModelledCosts =
                    new Cost
                    {
                        LevyCostOfTraining = 0,
                        LevyCompletionPayments = 0,
                        TransferOutCostOfTraining = 1100,
                        TransferOutCompletionPayments = 0
                    };

                // Adding LevyIn and removeing Projected Funds out to get the FundsOur as it would be calculated and stored in the DB.
                projectedFutureFunds += 2000 - estimationProjection.ActualCosts.FundsOut;
                estimationProjection.ProjectedFutureFunds = projectedFutureFunds;

                estimationProjection.Month = (short)date.Month;
                estimationProjection.Year = (short)date.Year;
                date = date.AddMonths(1);
            }

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(estimationProjectionList.AsReadOnly);

            // Act
            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            var first = actual.AccountFunds.Records.First();
            var second = actual.AccountFunds.Records.Skip(1).First();

            // Assert
            actual.AccountFunds.OpeningBalance.Should().Be(10_000);

            first.ActualCost.Should().Be(1300);
            first.EstimatedCost.Should().Be(1100);
            first.Balance.Should().Be(8900);

            second.ActualCost.Should().Be(3100, because: "actual costs = £800 (training) + £500 (transfer) + £1,000 (compl training) + £800 (compl transfer) = £3,100");
            second.EstimatedCost.Should().Be(1100);
            second.Balance.Should()
                .Be(6700,
                    because: "estimated balance = £8,900 (prev balance) + £2,000 (funds in) - £3,100 (actual out) - £1,100 (estimate out) = £6,700");
        }

        [Test]
        public async Task AccountFunds_Should_be_48_records()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            fixture.Customize<AccountEstimationProjectionModel>(m =>
                m.With(p => p.Month, 6)
                 .With(p => p.Year, (short)DateTime.Today.Year));

            var estimationProjectionList =
                fixture.CreateMany<AccountEstimationProjectionModel>(48)
                .ToList();

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(estimationProjectionList.AsReadOnly);

            // Act
            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            actual.AccountFunds.Records.Count().Should().Be(48);
        }

        public async Task Then_The_IsLessThanCost_Flag_Should_Be_False_If_You_Still_Have_Funds()
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

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsFalse(actual.TransferAllowances.First().IsLessThanCost);
        }

        [Test]
        public async Task Then_The_IsLessThanCost_Flag_Should_Be_False_If_You_Have_Zero_Funds()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) DateTime.Now.AddYears(1).Year,
                    Month = (short) DateTime.Now.Month,
                    ModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 50},
                    FutureFunds = 0
                }
            };

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsFalse(actual.TransferAllowances.First().IsLessThanCost);

        }
    }
}