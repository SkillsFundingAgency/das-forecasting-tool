using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        public ILevyPeriodRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public AllowAccountProjectionsHandler(ILevyPeriodRepository repository, ILog logger, IApplicationConfiguration applicationConfiguration)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        }

        public async Task<bool> Allow(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
        {
            Logger.Debug($"Now checking if projections can be generated for levy declaration events: {levySchemeDeclaration.ToDebugJson()}");
            if (levySchemeDeclaration.PayrollMonth == null)
                throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");
            if (!ApplicationConfiguration.AllowTriggerProjections)
            {
                Logger.Warn("Triggering of projections is disabled.");
                return false;
            }
            var levyPeriod = await Repository.Get(levySchemeDeclaration.AccountId, levySchemeDeclaration.PayrollYear,
                levySchemeDeclaration.PayrollMonth.Value);
            var lastReceivedTime = levyPeriod.GetLastTimeReceivedLevy();
            if (lastReceivedTime == null)
            {
                Logger.Warn($"No levy recorded for employer: {levySchemeDeclaration.AccountId}, period: {levySchemeDeclaration.PayrollYear}, {levySchemeDeclaration.PayrollMonth.Value}");
                return false;
            }
                
            var allowProjections = lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowProjections) <= DateTime.UtcNow;
            Logger.Info($"Allow projections '{allowProjections}' for employer '{levySchemeDeclaration.AccountId}' in response to levy event.");
            return allowProjections;
        }
    }
}