using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Services;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public class CurrentBalance
    {
        private readonly Models.Balance.BalanceModel _balance;
        private readonly IAccountBalanceService _accountBalanceService;
        private readonly ICommitmentsDataService _commitmentsDataService;

        public virtual long EmployerAccountId => _balance.EmployerAccountId;
        public virtual decimal Amount => _balance.Amount;
        public decimal TransferAllowance => _balance.TransferAllowance;
        public decimal RemainingTransferBalance => _balance.RemainingTransferBalance;
        public DateTime Period => _balance.BalancePeriod;
        public DateTime ReceivedDate => _balance.ReceivedDate;
        public decimal UnallocatedCompletionPayments => _balance.UnallocatedCompletionPayments;

        protected CurrentBalance() { }

        public CurrentBalance(
            Models.Balance.BalanceModel balance, 
            IAccountBalanceService accountBalanceService,
            ICommitmentsDataService commitmentsDataService)
        {
            _balance = balance ?? throw new ArgumentNullException(nameof(balance));
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
            _commitmentsDataService = commitmentsDataService ?? throw new ArgumentNullException(nameof(commitmentsDataService));
        }

        public virtual async Task<bool> RefreshBalance()
        {
            if (Period >= DateTime.UtcNow.AddDays(-1))
                return false;

            var currentBalance = await _accountBalanceService.GetAccountBalance(EmployerAccountId);
            if (currentBalance == null)
                throw new InvalidOperationException($"Failed to get the account balances for account: {EmployerAccountId}");

            var commitments = await _commitmentsDataService.GetCurrentCommitments(EmployerAccountId);
            var unallocatedCompletionPayments = 
                commitments
                .Where(m => m.PlannedEndDate < DateTime.Now.GetStartOfMonth())
                .Sum(m => m.CompletionAmount);

            _balance.Amount = currentBalance.Amount;
            _balance.TransferAllowance = currentBalance.TransferAllowance;
            _balance.RemainingTransferBalance = currentBalance.RemainingTransferBalance;
            _balance.BalancePeriod = DateTime.UtcNow;
            _balance.ReceivedDate = DateTime.UtcNow;
            _balance.UnallocatedCompletionPayments = unallocatedCompletionPayments;
            return true;
        }
    }
}