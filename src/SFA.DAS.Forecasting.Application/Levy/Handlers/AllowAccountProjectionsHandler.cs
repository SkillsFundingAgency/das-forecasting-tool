using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers
{
    public class AllowAccountProjectionsHandler
    {
        public ILevyPeriodRepository Repository { get; }
        public IAppInsightsTelemetry Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public AllowAccountProjectionsHandler(ILevyPeriodRepository repository, IAppInsightsTelemetry logger, IApplicationConfiguration applicationConfiguration)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        }

	    public async Task<bool> Allow(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
	    {
		    Logger.Info("LevyDeclarationAllowProjectionFunction",
			    $"Now checking if projections can be generated for levy declaration events: {levySchemeDeclaration.ToDebugJson()}",
			    "Allow");

		    if (levySchemeDeclaration.PayrollMonth == null)
			{
				Logger.Error("LevyDeclarationAllowProjectionFunction",
					new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: "),
					$"Received invalid levy declaration. No month specified. Data: {levySchemeDeclaration.ToJson()}",
					"Allow");
				throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");
			}

			if (!ApplicationConfiguration.AllowTriggerProjections)
            {
				Logger.Warning("LevyDeclarationAllowProjectionFunction", "Triggering of projections is disabled.", "Allow");
                return false;
            }

            var levyPeriod = await Repository.Get(levySchemeDeclaration.AccountId, levySchemeDeclaration.PayrollYear, levySchemeDeclaration.PayrollMonth.Value);
            var lastReceivedTime = levyPeriod.GetLastTimeReceivedLevy();
            if (lastReceivedTime == null)
            {
				Logger.Warning("LevyDeclarationAllowProjectionFunction", $"No levy recorded for employer: {levySchemeDeclaration.AccountId}, period: {levySchemeDeclaration.PayrollYear}, {levySchemeDeclaration.PayrollMonth.Value}", "Allow");
                return false;
            }
                
            var allowProjections = lastReceivedTime.Value.AddSeconds(ApplicationConfiguration.SecondsToWaitToAllowProjections) <= DateTime.UtcNow;
			Logger.Info("LevyDeclarationAllowProjectionFunction", $"Allow projections '{allowProjections}' for employer '{levySchemeDeclaration.AccountId}' in response to levy event.", "Allow");
            return allowProjections;
        }
    }
}