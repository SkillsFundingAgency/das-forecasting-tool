using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Balance.Services;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class BalanceDataService : IBalanceDataService
    {
        private readonly IForecastingDataContext _forecastingDataContext;
        private readonly ITelemetry _telemetry;

        public BalanceDataService(IForecastingDataContext forecastingDataContext, ITelemetry telemetry)
        {
            _forecastingDataContext = forecastingDataContext ?? throw new ArgumentNullException(nameof(forecastingDataContext));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<Models.Balance.BalanceModel> Get(long employerAccountId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var startTime = DateTime.UtcNow;
            var accountBalance = await _forecastingDataContext
                .Balances
                .FirstOrDefaultAsync(balance => balance.EmployerAccountId == employerAccountId);
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseQuery, "Get Balance", startTime, stopwatch.Elapsed, accountBalance != null);
            return accountBalance;
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var startTime = DateTime.UtcNow;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _forecastingDataContext.SaveChangesAsync();
                scope.Complete();
            }
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.SqlDatabaseUpdate, "Store Balance", startTime, stopwatch.Elapsed, true);
        }
    }
}