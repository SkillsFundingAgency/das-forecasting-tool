using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
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
	    private AutoMoqer _mocker;
	    private IAccountEstimationProjection _accountEstimationProjection;
		private const long ExpectedAccountId = 654311;
        private DateTime _dateFrom = DateTime.Now.AddYears(1);

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
                .Returns(new List<AccountEstimationProjectionModel>().AsReadOnly);

            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.TransferAllowance)
                .Returns(400m);

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
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom .Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        TransferOutCostOfTraining =  50,
                        TransferOutCompletionPayments = 50,
                    },
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        TransferOutCostOfTraining =  50,
                        TransferOutCompletionPayments = 50,
                    }
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        TransferOutCostOfTraining =  actualTotalCostOfTraining,
                        TransferOutCompletionPayments = actualCommittedCompletionPayments,
                    },
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost
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

            Assert.AreEqual(actualTotalCostOfTraining + actualCommittedCompletionPayments, actual.TransferAllowances.Records.First().ActualCost);
            Assert.AreEqual(transferOutTotalCostOfTraining + transferOutCompletionPayment, actual.TransferAllowances.Records.First().EstimatedCost);
        }

        [Test]
        public async Task Then_The_Levy_Out_Costs_Are_Not_factored_Into_The_Acutal_Costs()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 50M,
                        LevyCompletionPayments = 50M,
                        TransferOutCostOfTraining =  25m,
                        TransferOutCompletionPayments = 25m
                    }
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
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

            Assert.AreEqual(100m,actual.TransferAllowances.Records.First().ActualCost);

        }

        [Test]
        public async Task Then_The_Estimated_Cost_Includes_All_Estimations()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 50m,
                        LevyCompletionPayments = 50m,
                        TransferOutCostOfTraining =  25M,
                        TransferOutCompletionPayments = 25M
                    },
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 200M,
                        LevyCompletionPayments = 200M,
                        TransferOutCostOfTraining =  20M,
                        TransferOutCompletionPayments = 20m
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 400m,
                        LevyCompletionPayments = 400m,
                        TransferOutCostOfTraining =  40m,
                        TransferOutCompletionPayments = 40m
                    }
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 100m,
                        LevyCompletionPayments = 100m,
                        TransferOutCostOfTraining =  50m,
                        TransferOutCompletionPayments = 50m
                    },
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 400m,
                        LevyCompletionPayments = 400m,
                        TransferOutCostOfTraining =  40m,
                        TransferOutCompletionPayments = 40m
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 800m,
                        LevyCompletionPayments = 800m,
                        TransferOutCostOfTraining =  80m,
                        TransferOutCompletionPayments = 80m
                    }
                }
            };

            //Act
            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            //Assert
            Assert.AreEqual(1760m, actual.AccountFunds.Records.First().EstimatedCost);
            Assert.AreEqual(300m, actual.AccountFunds.Records.First().ActualCost);
        }


        [Test]
        public async Task Then_The_Transfer_Allowance_Inclues_Only_Transfer_Modelled_Costs()
        {
            //Arrange
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 50m,
                        LevyCompletionPayments = 50m,
                        TransferOutCostOfTraining =  25M,
                        TransferOutCompletionPayments = 25M
                    },
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 200M,
                        LevyCompletionPayments = 200M,
                        TransferOutCostOfTraining =  20M,
                        TransferOutCompletionPayments = 20m
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 400m,
                        LevyCompletionPayments = 400m,
                        TransferOutCostOfTraining =  40m,
                        TransferOutCompletionPayments = 40m
                    }
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
                    ActualCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 100m,
                        LevyCompletionPayments = 100m,
                        TransferOutCostOfTraining =  50m,
                        TransferOutCompletionPayments = 50m
                    },
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 400m,
                        LevyCompletionPayments = 400m,
                        TransferOutCostOfTraining =  40m,
                        TransferOutCompletionPayments = 40m
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost
                    {
                        LevyCostOfTraining = 800m,
                        LevyCompletionPayments = 800m,
                        TransferOutCostOfTraining =  80m,
                        TransferOutCompletionPayments = 80m
                    }
                }
            };

            //Act
            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            //Assert
            Assert.AreEqual(80m, actual.TransferAllowances.Records.First().EstimatedCost);
            Assert.AreEqual(100m, actual.TransferAllowances.Records.First().ActualCost);
        }

        [Test]
        public async Task Then_The_IsLessThanCost_Flag_Uses_AvailableTransferFundsBalance()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 50},
                    AvailableTransferFundsBalance = -100
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 50},
                    AvailableTransferFundsBalance = -100
                }
            };

			_mocker.GetMock<IAccountEstimationProjection>()
				.Setup(x => x.Projections)
				.Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsTrue(actual.TransferAllowances.Records.First().IsLessThanCost);
        }

        [Test]
        public async Task Then_The_IsLessThanCost_Flag_Should_Be_False_If_You_Still_Have_Funds()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 30},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 25},
                    AvailableTransferFundsBalance = 50
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 50},
                    AvailableTransferFundsBalance = 100
                }
            };

			_mocker.GetMock<IAccountEstimationProjection>()
				.Setup(x => x.Projections)
				.Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsFalse(actual.TransferAllowances.Records.First().IsLessThanCost);
        }

        [Test]
        public async Task Then_The_IsLessThanCost_Flag_Should_Be_False_If_You_Have_Zero_Funds()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 30},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 25},
                    AvailableTransferFundsBalance = 0
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
                    TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                    ActualCosts = new AccountEstimationProjectionModel.Cost{ TransferOutCostOfTraining = 50},
                    AvailableTransferFundsBalance = 0
                }
            };

			_mocker.GetMock<IAccountEstimationProjection>()
				.Setup(x => x.Projections)
				.Returns(expectedAccountEstimationProjectionList.AsReadOnly);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            Assert.IsFalse(actual.TransferAllowances.Records.First().IsLessThanCost);
        }

        [Test]
        public async Task Then_Balance_Values_Less_Than_Or_Equal_To_Zero_Are_Not_Shown_In_The_Model()
        {
            var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>
            {
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.Year,
                    Month = (short) _dateFrom.Month,
                    EstimatedProjectionBalance = 10
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(1).Year,
                    Month = (short) _dateFrom.AddMonths(1).Month,
                    EstimatedProjectionBalance = 1
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(2).Year,
                    Month = (short) _dateFrom.AddMonths(2).Month,
                    EstimatedProjectionBalance = 0
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(3).Year,
                    Month = (short) _dateFrom.AddMonths(3).Month,
                    EstimatedProjectionBalance = -10
                },
                new AccountEstimationProjectionModel
                {
                    Year = (short) _dateFrom.AddMonths(3).Year,
                    Month = (short) _dateFrom.AddMonths(3).Month,
                    EstimatedProjectionBalance = -20
                }
            };

            //Act
            _mocker.GetMock<IAccountEstimationProjection>()
                .Setup(x => x.Projections)
                .Returns(expectedAccountEstimationProjectionList.AsReadOnly);
            _mocker.GetMock<IAccountEstimationProjectionRepository>()
                .Setup(x => x.Get(It.IsAny<AccountEstimation>()))
                .ReturnsAsync(_accountEstimationProjection);

            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            //Assert
            Assert.AreEqual("£1", actual.AccountFunds.Records[0].FormattedBalance);
            Assert.AreEqual("-", actual.AccountFunds.Records[1].FormattedBalance);
            Assert.AreEqual("-", actual.AccountFunds.Records[2].FormattedBalance);
            Assert.AreEqual("-", actual.AccountFunds.Records[3].FormattedBalance);
        }

        [Test]
        public async Task Then_The_Remaining_Transfer_Allowance_Is_Populated()
        {
            //Act
            var actual = await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);

            //Assert
            Assert.AreEqual(400m, actual.TransferAllowances.AnnualTransferAllowance);
        }
    }
}