using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance.Services;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public class CurrentBalance
    {
        private readonly Models.Balance.BalanceModel _balance;
        private readonly IAccountBalanceService _accountBalanceService;
        public virtual long EmployerAccountId => _balance.EmployerAccountId;
        public virtual decimal Amount => _balance.Amount;
        public decimal TransferAllowance => _balance.TransferAllowance;
        public decimal RemainingTransferBalance => _balance.RemainingTransferBalance;
        public DateTime Period => _balance.BalancePeriod;
        public DateTime ReceivedDate => _balance.ReceivedDate;
        public decimal UnallocatedCompletionPayments => _balance.UnallocatedCompletionPayments;

        protected CurrentBalance() { }

        public CurrentBalance(Models.Balance.BalanceModel balance, IAccountBalanceService accountBalanceService)
        {
            _balance = balance ?? throw new ArgumentNullException(nameof(balance));
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
        }

        public virtual async Task<bool> RefreshBalance()
        {
            if (Period > DateTime.UtcNow.AddMonths(-1))
                return false;

            var currentBalance = await _accountBalanceService.GetAccountBalance(EmployerAccountId);
            if (currentBalance == null)
                throw new InvalidOperationException($"Failed to get the account balances for account: {EmployerAccountId}");

            _balance.Amount = currentBalance.Amount;
            _balance.TransferAllowance = currentBalance.TransferAllowance;
            _balance.RemainingTransferBalance = currentBalance.RemainingTransferBalance;
            _balance.BalancePeriod = DateTime.UtcNow;
            _balance.ReceivedDate = DateTime.UtcNow;
            _balance.UnallocatedCompletionPayments = 1.4M; // ToDo: calculate UnallocatedCompletionPayments
            return true;
        }
    }
}