using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Balance.Services;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class BalanceDataService: IBalanceDataService
    {
        private readonly IForecastingDataContext _forecastingDataContext;

        public BalanceDataService(IForecastingDataContext  forecastingDataContext)
        {
            _forecastingDataContext = forecastingDataContext ?? throw new ArgumentNullException(nameof(forecastingDataContext));
        }

        public async Task<Models.Balance.BalanceModel> Get(long employerAccountId)
        {
            return await _forecastingDataContext
                .Balances
                .FirstOrDefaultAsync(balance => balance.EmployerAccountId == employerAccountId);
        }

        public async Task Store(Models.Balance.BalanceModel balance)
        {
            var persistedBalance = await Get(balance.EmployerAccountId);
            if (persistedBalance != null)
            {
                persistedBalance.Amount = balance.Amount;
                persistedBalance.BalancePeriod = balance.BalancePeriod;
                persistedBalance.ReceivedDate = balance.ReceivedDate;
                persistedBalance.RemainingTransferBalance = balance.RemainingTransferBalance;
                persistedBalance.TransferAllowance = balance.TransferAllowance;
                persistedBalance.UnallocatedCompletionPayments = balance.UnallocatedCompletionPayments;
            }
            else
            {
                if (balance.ReceivedDate != DateTime.MinValue && balance.BalancePeriod != DateTime.MinValue)
                {
                    _forecastingDataContext.Balances.Add(balance);
                }
            }
                
            await _forecastingDataContext.SaveChangesAsync();
        }
    }
}