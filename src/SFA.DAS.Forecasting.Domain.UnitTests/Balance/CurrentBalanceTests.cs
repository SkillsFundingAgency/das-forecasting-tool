using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Balance;

[TestFixture]
public class CurrentBalanceTests
{
    private BalanceModel _currentAccountBalance;
    private Mock<IAccountBalanceService> _accountBalanceService;
    private EmployerCommitments _employerCommitments;
    private CurrentBalance _currentBalance;

    [SetUp]
    public void SetUp()
    {
        _currentAccountBalance = new BalanceModel
        {
            EmployerAccountId = 12345,
            BalancePeriod = DateTime.UtcNow.AddDays(-1),
            Amount = 10000,
            TransferAllowance = 1000,
            RemainingTransferBalance = 1000
        };
        _employerCommitments = new EmployerCommitments(12345, new EmployerCommitmentsModel{ LevyFundedCommitments = new List<CommitmentModel>
            {
                new()
                {
                    EmployerAccountId = 12345,
                    PlannedEndDate = DateTime.Today.AddMonths(-2),
                    StartDate = DateTime.Today.AddMonths(-3),
                    NumberOfInstallments = 1,
                    CompletionAmount = 1000,
                    MonthlyInstallment = 80,
                    HasHadPayment = true,
                    FundingSource = FundingSource.Levy
                },
                new()
                {
                    EmployerAccountId = 12345,
                    PlannedEndDate = DateTime.Today.AddMonths(-1),
                    StartDate = DateTime.Today.AddMonths(-3),
                    NumberOfInstallments = 1,
                    CompletionAmount = 1000,
                    MonthlyInstallment = 80,
                    HasHadPayment = true,
                    FundingSource = FundingSource.Levy
                }
            }
        });
            

        _accountBalanceService = new Mock<IAccountBalanceService>();
        _accountBalanceService.Setup(x => x.GetAccountBalance(It.IsAny<long>()))
            .ReturnsAsync(_currentAccountBalance);
        
        _currentBalance = new CurrentBalance(_currentAccountBalance, _accountBalanceService.Object,
            _employerCommitments);
    }

    [Test]
    public async Task Refresh_Balance_Gets_Balance_From_Accounts_Api()
    {
        Assert.IsTrue(await _currentBalance.RefreshBalance(), "Failed to update the current employer balance.");
        _accountBalanceService.Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _currentBalance.EmployerAccountId)));
        Assert.AreEqual(_currentAccountBalance.Amount, _currentBalance.Amount);
    }

    [Test]
    public async Task Refresh_Balance_Refreshes_Transfer_Allowance()
    {
        Assert.IsTrue(await _currentBalance.RefreshBalance(), "Failed to update the current employer balance.");
        _accountBalanceService.Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _currentBalance.EmployerAccountId)));
        Assert.AreEqual(_currentAccountBalance.TransferAllowance, _currentBalance.TransferAllowance);
    }

    [Test]
    public async Task Refresh_Balance_Refreshes_Remaining_Transfer_Allowance()
    {
        Assert.IsTrue(await _currentBalance.RefreshBalance(), "Failed to update the current employer balance.");
        _accountBalanceService.Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _currentBalance.EmployerAccountId)));
        Assert.AreEqual(_currentAccountBalance.RemainingTransferBalance, _currentBalance.RemainingTransferBalance);
    }

    [Test]
    public async Task Refresh_Balance_Refreshes_unallocated_completion_payments()
    {
        Assert.IsTrue(await _currentBalance.RefreshBalance(), "Failed to update the current employer balance.");
        Assert.AreEqual(0, _currentBalance.UnallocatedCompletionPayments);
    }


    [Test]
    public async Task Refresh_Balance_Refreshes_unallocated_completion_payments_when_rebuilding_projection()
    {
        Assert.IsTrue(await _currentBalance.RefreshBalance(true), "Failed to update the current employer balance.");
        Assert.AreEqual(1000, _currentBalance.UnallocatedCompletionPayments);
    }

    [Test]
    public async Task Refresh_Balance_Refreshes_unallocated_completion_payments_include_unpaid_when_rebuilding_projection()
    {
        Assert.IsTrue(await _currentBalance.RefreshBalance(true,true), "Failed to update the current employer balance.");
        Assert.AreEqual(1000, _currentBalance.UnallocatedCompletionPayments);
    }


    [Test]
    public async Task Does_Not_Refresh_If_Last_Refresh_Less_Than_1_Day_Ago()
    {
        _currentAccountBalance.BalancePeriod = DateTime.UtcNow.AddHours(-23);
        Assert.IsFalse(await _currentBalance.RefreshBalance(), "Updated the current employer balance.");
        _accountBalanceService
            .Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _currentBalance.EmployerAccountId)), Times.Never());
    }
}