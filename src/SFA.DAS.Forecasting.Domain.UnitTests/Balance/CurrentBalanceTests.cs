using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Balance;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Balance
{
    [TestFixture]
    public class CurrentBalanceTests
    {
        private CurrentBalance _currentBalance;
        private Models.Balance.Balance _balance;

        [SetUp]
        public void SetUp()
        {
            _balance = new Models.Balance.Balance
            {
                EmployerAccountId = 12345,
                BalancePeriod = DateTime.Today,
                Amount = 10000
            };
            _currentBalance = new CurrentBalance(_balance);
        }

        [Test]
        public void Rejects_Balances_Generated_Prior_To_Current_Balance()
        {
            Assert.IsFalse(_currentBalance.SetCurrentBalance(10000,_balance.BalancePeriod.AddDays(-1)));
        }

        [Test]
        public void Allows_Balances_Generated_After_Current_Balance()
        {
            Assert.IsTrue(_currentBalance.SetCurrentBalance(10000, _balance.BalancePeriod.AddDays(1)));
        }
    }
}