using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance.Services;

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

        public CurrentBalanceRepository(IBalanceDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }
        public async Task<CurrentBalance> Get(long employerAccountId)
        {
            var employerAccount = await _dataService.Get(employerAccountId) ?? new Models.Balance.Balance { EmployerAccountId = employerAccountId, BalancePeriod = DateTime.MinValue};
            return new CurrentBalance(employerAccount);
        }

        public async Task Store(CurrentBalance currentBalance)
        {
            await _dataService.Store(new Models.Balance.Balance
            {
                EmployerAccountId = currentBalance.EmployerAccountId,
                BalancePeriod = currentBalance.Period,
                ReceivedDate = currentBalance.ReceivedDate,
                Amount = currentBalance.Amount
            });
        }
    }
}