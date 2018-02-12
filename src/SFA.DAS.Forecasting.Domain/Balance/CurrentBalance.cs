using System;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public class CurrentBalance
    {
        private readonly Models.Balance.Balance _balance;
        public long EmployerAccountId => _balance.EmployerAccountId;
        public decimal Amount => _balance.Amount;
        public DateTime Period => _balance.BalancePeriod;
        public DateTime ReceivedDate => _balance.ReceivedDate;

        public CurrentBalance(Models.Balance.Balance balance)
        {
            _balance = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public bool SetCurrentBalance(decimal amount, DateTime balancePeriod)
        {
            if (balancePeriod < _balance.BalancePeriod)
                return false;
            _balance.Amount = amount;
            _balance.BalancePeriod = balancePeriod;
            _balance.ReceivedDate = DateTime.UtcNow;
            return true;
        }
    }
}