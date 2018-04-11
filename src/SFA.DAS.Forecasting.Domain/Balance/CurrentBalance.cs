using System;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public class CurrentBalance
    {
        private readonly Models.Balance.Balance _balance;
        public long EmployerAccountId => _balance.EmployerAccountId;
        public decimal Amount => _balance.Amount;
        public decimal TransferAllowance => _balance.TransferAllowance;
        public decimal RemainingTransferBalance => _balance.RemainingTransferBalance;
        public DateTime Period => _balance.BalancePeriod;
        public DateTime ReceivedDate => _balance.ReceivedDate;

        public CurrentBalance(Models.Balance.Balance balance)
        {
            _balance = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public bool SetCurrentBalance(decimal amount, decimal transferAllowance, decimal remainingTransferBalance, DateTime balancePeriod)
        {
            if (balancePeriod < _balance.BalancePeriod)
                return false;
            _balance.Amount = amount;
            _balance.BalancePeriod = balancePeriod;
            _balance.ReceivedDate = DateTime.UtcNow;
            _balance.TransferAllowance = transferAllowance;
            _balance.RemainingTransferBalance = remainingTransferBalance;
            return true;
        }
    }
}