using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public class CurrentBalance
    {
        internal Models.Balance.BalanceModel Model { get; private set; }
        private readonly IAccountBalanceService _accountBalanceService;
        protected internal readonly EmployerCommitments EmployerCommitments;

        public virtual long EmployerAccountId => Model.EmployerAccountId;
        public virtual decimal Amount => Model.Amount;
        public decimal TransferAllowance => Model.TransferAllowance;
        public decimal RemainingTransferBalance => Model.RemainingTransferBalance;
        public DateTime Period => Model.BalancePeriod;
        public DateTime ReceivedDate => Model.ReceivedDate;
        public decimal UnallocatedCompletionPayments => Model.UnallocatedCompletionPayments;

        protected CurrentBalance() { }

        public CurrentBalance(
            Models.Balance.BalanceModel balance, 
            IAccountBalanceService accountBalanceService,
            EmployerCommitments employerCommitments)
        {
            Model = balance ?? throw new ArgumentNullException(nameof(balance));
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
            EmployerCommitments = employerCommitments ?? throw new ArgumentNullException(nameof(employerCommitments));
        }

        public virtual async Task<bool> RefreshBalance(bool refreshUnallocatedCompletionPayments = false, bool includeUnpaidCompletionPayments = false)
        {
            if (Period >= DateTime.UtcNow.AddDays(-1))
                return false;

            var currentBalance = await _accountBalanceService.GetAccountBalance(EmployerAccountId);
            if (currentBalance == null)
            {
                throw new InvalidOperationException($"Failed to get the account balances for account: {EmployerAccountId}");
            }

            Model.Amount = currentBalance.Amount;
            Model.TransferAllowance = currentBalance.TransferAllowance;
            Model.RemainingTransferBalance = currentBalance.RemainingTransferBalance;
            Model.BalancePeriod = DateTime.UtcNow;
            Model.ReceivedDate = DateTime.UtcNow;
            if (!refreshUnallocatedCompletionPayments)
            {
                return true;
            }
            var unallocatedCompletionPayments = EmployerCommitments.GetUnallocatedCompletionAmount(includeUnpaidCompletionPayments);
            Model.UnallocatedCompletionPayments = unallocatedCompletionPayments;
            return true;
        }
    }
}