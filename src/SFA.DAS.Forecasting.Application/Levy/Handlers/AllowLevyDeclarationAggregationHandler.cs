using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Levy.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers
{
    public class AllowLevyDeclarationAggregationHandler
    {
        public ILevyPeriodRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public AllowLevyDeclarationAggregationHandler(ILevyPeriodRepository repository, ILog logger, IApplicationConfiguration applicationConfiguration)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        }

        public async Task<bool> Allow(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
        {
            Logger.Debug($"Now checking if we can aggregate levy declarations for event: {levySchemeDeclaration.ToDebugJson()}");
            if (levySchemeDeclaration.PayrollMonth == null)
                throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");
            var levyPeriod = await Repository.Get(levySchemeDeclaration.AccountId, levySchemeDeclaration.PayrollYear,
                levySchemeDeclaration.PayrollMonth.Value);
            var lastReceivedTime = levyPeriod.GetLastTimeReceivedLevy();
            if (lastReceivedTime == null)
                throw new InvalidOperationException($"Invalid last time received");
            return lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowAggregation) <= DateTime.Now;
        }
    }
}