using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Models.Projections;
using AccountEstimation = SFA.DAS.Forecasting.Domain.Estimations.AccountEstimation;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Web.UnitTests.OrchestratorTests;

public class EstimationOrchestratorTests
{
    private EstimationOrchestrator _orchestrator;
    private Mock<IAccountEstimationProjection> _accountEstimationProjection;
    private const long ExpectedAccountId = 654311;
    private readonly DateTime _dateFrom = DateTime.Now.AddYears(1);
    private Mock<IEncodingService> _hashingService;
    private Mock<CurrentBalance> _currentBalance;
    private Mock<ICurrentBalanceRepository> _currentBalanceRepository;
    private Mock<IAccountEstimationProjectionRepository> _accountEstimationProjectionRepository;
    private Mock<IExpiredFundsService> _expiredFundsService;

    [SetUp]
    public void Arrange()
    {
        _hashingService = new Mock<IEncodingService>();
        _hashingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.AccountId))
            .Returns(ExpectedAccountId);

        _currentBalance = new Mock<CurrentBalance>();
        _currentBalance.Setup(x => x.RefreshBalance(It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(true);

        _currentBalanceRepository = new Mock<ICurrentBalanceRepository>();
        _currentBalanceRepository.Setup(x => x.Get(ExpectedAccountId))
            .ReturnsAsync(_currentBalance.Object);

        _accountEstimationProjection = new Mock<IAccountEstimationProjection>();
        _accountEstimationProjection.Setup(x => x.Projections)
            .Returns(new List<AccountEstimationProjectionModel>().AsReadOnly);

        _accountEstimationProjection
            .Setup(x => x.TransferAllowance)
            .Returns(400m);
        
        _accountEstimationProjectionRepository = new Mock<IAccountEstimationProjectionRepository>();
        _accountEstimationProjectionRepository.Setup(x => x.Get(It.IsAny<AccountEstimation>(),false))
            .ReturnsAsync(_accountEstimationProjection.Object);

        _expiredFundsService = new Mock<IExpiredFundsService>();
        _expiredFundsService.Setup(s => s.GetExpiringFunds(It.IsAny<ReadOnlyCollection<AccountEstimationProjectionModel>>(),
            It.IsAny<long>(),It.IsAny<ProjectionGenerationType>(), It.IsAny<DateTime>())).ReturnsAsync(new Dictionary<CalendarPeriod, decimal>());

        _orchestrator = new EstimationOrchestrator(_accountEstimationProjectionRepository.Object,
            Mock.Of<IAccountEstimationRepository>(), _hashingService.Object, _currentBalanceRepository.Object,
            Mock.Of<IApprenticeshipCourseDataService>(), _expiredFundsService.Object);
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    TransferOutCostOfTraining = 50,
                    TransferOutCompletionPayments = 50,
                },
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    TransferOutCostOfTraining = 50,
                    TransferOutCompletionPayments = 50,
                }
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    TransferOutCostOfTraining = actualTotalCostOfTraining,
                    TransferOutCompletionPayments = actualCommittedCompletionPayments,
                },
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    TransferOutCostOfTraining = transferOutTotalCostOfTraining,
                    TransferOutCompletionPayments = transferOutCompletionPayment,
                }
            }
        };

        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 50M,
                    LevyCompletionPayments = 50M,
                    TransferOutCostOfTraining = 25m,
                    TransferOutCompletionPayments = 25m
                }
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 100m,
                    LevyCompletionPayments = 100m,
                    TransferOutCostOfTraining = 50m,
                    TransferOutCompletionPayments = 50m
                }
            }
        };

        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 50m,
                    LevyCompletionPayments = 50m,
                    TransferOutCostOfTraining = 25M,
                    TransferOutCompletionPayments = 25M
                },
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 200M,
                    LevyCompletionPayments = 200M,
                    TransferOutCostOfTraining = 20M,
                    TransferOutCompletionPayments = 20m
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 400m,
                    LevyCompletionPayments = 400m,
                    TransferOutCostOfTraining = 40m,
                    TransferOutCompletionPayments = 40m
                }
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 100m,
                    LevyCompletionPayments = 100m,
                    TransferOutCostOfTraining = 50m,
                    TransferOutCompletionPayments = 50m
                },
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 400m,
                    LevyCompletionPayments = 400m,
                    TransferOutCostOfTraining = 40m,
                    TransferOutCompletionPayments = 40m
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 800m,
                    LevyCompletionPayments = 800m,
                    TransferOutCostOfTraining = 80m,
                    TransferOutCompletionPayments = 80m
                }
            }
        };

        //Act
        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 50m,
                    LevyCompletionPayments = 50m,
                    TransferOutCostOfTraining = 25M,
                    TransferOutCompletionPayments = 25M
                },
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 200M,
                    LevyCompletionPayments = 200M,
                    TransferOutCostOfTraining = 20M,
                    TransferOutCompletionPayments = 20m
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 400m,
                    LevyCompletionPayments = 400m,
                    TransferOutCostOfTraining = 40m,
                    TransferOutCompletionPayments = 40m
                }
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 100m,
                    LevyCompletionPayments = 100m,
                    TransferOutCostOfTraining = 50m,
                    TransferOutCompletionPayments = 50m
                },
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 400m,
                    LevyCompletionPayments = 400m,
                    TransferOutCostOfTraining = 40m,
                    TransferOutCompletionPayments = 40m
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = 800m,
                    LevyCompletionPayments = 800m,
                    TransferOutCostOfTraining = 80m,
                    TransferOutCompletionPayments = 80m
                }
            }
        };

        //Act
        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                ActualCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 50},
                AvailableTransferFundsBalance = -100
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                ActualCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 50},
                AvailableTransferFundsBalance = -100
            }
        };

        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 30},
                ActualCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 25},
                AvailableTransferFundsBalance = 50
            },
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                ActualCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 50},
                AvailableTransferFundsBalance = 100
            }
        };

        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 30},
                ActualCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 25},
                AvailableTransferFundsBalance = 0
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 60},
                ActualCosts = new AccountEstimationProjectionModel.Cost {TransferOutCostOfTraining = 50},
                AvailableTransferFundsBalance = 0
            }
        };

        _accountEstimationProjection
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
            new()
            {
                Year = (short) _dateFrom.Year,
                Month = (short) _dateFrom.Month,
                EstimatedProjectionBalance = 10
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(1).Year,
                Month = (short) _dateFrom.AddMonths(1).Month,
                EstimatedProjectionBalance = 1
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(2).Year,
                Month = (short) _dateFrom.AddMonths(2).Month,
                EstimatedProjectionBalance = 0
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(3).Year,
                Month = (short) _dateFrom.AddMonths(3).Month,
                EstimatedProjectionBalance = -10
            },
            new()
            {
                Year = (short) _dateFrom.AddMonths(3).Year,
                Month = (short) _dateFrom.AddMonths(3).Month,
                EstimatedProjectionBalance = -20
            }
        };

        //Act
        _accountEstimationProjection
            .Setup(x => x.Projections)
            .Returns(expectedAccountEstimationProjectionList.AsReadOnly);
        _accountEstimationProjectionRepository
            .Setup(x => x.Get(It.IsAny<AccountEstimation>(), It.IsAny<bool>()))
            .ReturnsAsync(_accountEstimationProjection.Object);

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
		
    [Test]
    public async Task Then_ExpiredFunds_Are_Calculated()
    {
        //Act
        var expectedAccountEstimationProjectionList = new List<AccountEstimationProjectionModel>()
        {
            new()
            {
                Month = 1,
                Year = 2018,
                EstimatedProjectionBalance = 100,
                FundsIn = 1000,
                ActualCosts = new AccountEstimationProjectionModel.Cost()
                {
                    LevyCompletionPayments = 0,
                    LevyCostOfTraining = 800
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost()
                {
                    LevyCostOfTraining = 100,
                    LevyCompletionPayments = 0
                }
            },
            new()
            {
                Month = 2,
                Year = 2018,
                EstimatedProjectionBalance = 400,
                FundsIn = 1000,
                ActualCosts = new AccountEstimationProjectionModel.Cost()
                {
                    LevyCompletionPayments = 0,
                    LevyCostOfTraining = 500
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost()
                {
                    LevyCostOfTraining = 200,
                    LevyCompletionPayments = 0
                }
            },
            new()
            {
                Month = 3,
                Year = 2018,
                EstimatedProjectionBalance = -1700,
                FundsIn = 1000,
                ActualCosts = new AccountEstimationProjectionModel.Cost()
                {
                    LevyCompletionPayments = 1000,
                    LevyCostOfTraining = 800
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost()
                {
                    LevyCostOfTraining = 300,
                    LevyCompletionPayments = 1000
                }
            },
        };

        var expiredFunds = new Dictionary<CalendarPeriod, decimal>()
        {
            { new CalendarPeriod(2018,2),100m}
        };

        _accountEstimationProjection
            .Setup(x => x.Projections)
            .Returns(expectedAccountEstimationProjectionList.AsReadOnly);

        _expiredFundsService
            .Setup(s => s.GetExpiringFunds(It.IsAny<ReadOnlyCollection<AccountEstimationProjectionModel>>(),
                It.IsAny<long>(),It.IsAny<ProjectionGenerationType>(), It.IsAny<DateTime>()))
            .ReturnsAsync(expiredFunds);

        await _orchestrator.CostEstimation("ABC123", "Test-Estimation", false);
        _accountEstimationProjection.Verify(v => v.ApplyExpiredFunds(expiredFunds),Times.Once);
    }
}