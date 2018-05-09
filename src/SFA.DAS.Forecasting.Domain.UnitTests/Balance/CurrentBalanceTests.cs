using System;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Balance
{
    [TestFixture]
    public class CurrentBalanceTests
    {
        private AutoMoqer _moqer;
        private CurrentBalance _currentBalance;
        private Models.Balance.BalanceModel _previousAccountBalance;
        private Models.Balance.BalanceModel _currentAccountBalance;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _previousAccountBalance = new BalanceModel
            {
                EmployerAccountId = 12345,
                BalancePeriod = DateTime.MinValue,
            };
            _currentAccountBalance = new BalanceModel
            {
                EmployerAccountId = 12345,
                BalancePeriod = DateTime.Today,
                Amount = 10000,
                TransferAllowance = 1000,
                RemainingTransferBalance = 1000
            };
            _moqer.SetInstance(_previousAccountBalance);
            _moqer.GetMock<IAccountBalanceService>()
                .Setup(svc => svc.GetAccountBalance(It.IsAny<long>()))
                .Returns(Task.FromResult<BalanceModel>(_currentAccountBalance));
        }

        [Test]
        public async Task Refresh_Balance_Gets_Balance_From_Accounts_Api()
        {
            var balance = _moqer.Resolve<CurrentBalance>();
            Assert.IsTrue(await balance.RefreshBalance(), "Failed to update the current employer balance.");
            _moqer.GetMock<IAccountBalanceService>()
                .Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _previousAccountBalance.EmployerAccountId)));
            Assert.AreEqual(_currentAccountBalance.Amount, balance.Amount);
        }

        [Test]
        public async Task Refresh_Balance_Refreshes_Balance()
        {
            var balance = _moqer.Resolve<CurrentBalance>();
            Assert.IsTrue(await balance.RefreshBalance(), "Failed to update the current employer balance.");
            _moqer.GetMock<IAccountBalanceService>()
                .Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _previousAccountBalance.EmployerAccountId)));
            Assert.AreEqual(_currentAccountBalance.Amount, balance.Amount);
        }

        [Test]
        public async Task Refresh_Balance_Refreshes_Transfer_Allowance()
        {
            var balance = _moqer.Resolve<CurrentBalance>();
            Assert.IsTrue(await balance.RefreshBalance(), "Failed to update the current employer balance.");
            _moqer.GetMock<IAccountBalanceService>()
                .Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _previousAccountBalance.EmployerAccountId)));
            Assert.AreEqual(_currentAccountBalance.TransferAllowance, balance.TransferAllowance);
        }

        [Test]
        public async Task Refresh_Balance_Refreshes_Remaining_Transfer_Allowance()
        {
            var balance = _moqer.Resolve<CurrentBalance>();
            Assert.IsTrue(await balance.RefreshBalance(), "Failed to update the current employer balance.");
            _moqer.GetMock<IAccountBalanceService>()
                .Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _previousAccountBalance.EmployerAccountId)));
            Assert.AreEqual(_currentAccountBalance.RemainingTransferBalance, balance.RemainingTransferBalance);
        }


        [Test]
        public async Task Does_Not_Refresh_If_Last_Refresh_Less_Than_1_Day_Ago()
        {
            _previousAccountBalance.BalancePeriod = DateTime.Today.AddHours(-23);
            var balance = _moqer.Resolve<CurrentBalance>();
            Assert.IsFalse(await balance.RefreshBalance(), "Updated the current employer balance.");
            _moqer.GetMock<IAccountBalanceService>()
                .Verify(svc => svc.GetAccountBalance(It.Is<long>(id => id == _previousAccountBalance.EmployerAccountId)), Times.Never());
        }
    }
}