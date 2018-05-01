using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Services;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public interface ICurrentBalanceRepository
    {
        Task<CurrentBalance> Get(long employerAccountId);
        Task Store(CurrentBalance currentBalance);
    }

    public class CurrentBalanceRepository : ICurrentBalanceRepository
    {
        private readonly IBalanceDataService _dataService;
        private readonly IAccountBalanceService _accountBalanceService;
        private readonly ICommitmentsDataService _commitmentsDataService;

        public CurrentBalanceRepository(
            IBalanceDataService dataService, 
            IAccountBalanceService accountBalanceService,
            ICommitmentsDataService commitmentsDataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
            _commitmentsDataService = commitmentsDataService;
        }
        public async Task<CurrentBalance> Get(long employerAccountId)
        {
            var employerAccount = await _dataService.Get(employerAccountId) ?? new Models.Balance.BalanceModel { EmployerAccountId = employerAccountId, BalancePeriod = DateTime.MinValue };
            return new CurrentBalance(employerAccount, _accountBalanceService, _commitmentsDataService);
        }

        public async Task Store(CurrentBalance currentBalance)
        {
            await _dataService.Store(new Models.Balance.BalanceModel
            {
                EmployerAccountId = currentBalance.EmployerAccountId,
                BalancePeriod = currentBalance.Period,
                ReceivedDate = currentBalance.ReceivedDate,
                Amount = currentBalance.Amount,
                TransferAllowance = currentBalance.TransferAllowance,
                RemainingTransferBalance = currentBalance.RemainingTransferBalance,
                UnallocatedCompletionPayments = currentBalance.UnallocatedCompletionPayments
            });
        }
    }
}