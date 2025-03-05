using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Core;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers;

public interface IAllowAccountProjectionsHandler
{
    Task<bool> Allow(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration);
}
public class AllowAccountProjectionsHandler : IAllowAccountProjectionsHandler
{
    private const int SecondsToWaitToAllowProjections = 60;
    private readonly IEmployerProjectionAuditService _auditService;
    private ILevyPeriodRepository Repository { get; }
    private ILogger<AllowAccountProjectionsHandler> Logger { get; }
    private ForecastingJobsConfiguration ApplicationConfiguration { get; }

    public AllowAccountProjectionsHandler(ILevyPeriodRepository repository, ILogger<AllowAccountProjectionsHandler> logger, ForecastingJobsConfiguration applicationConfiguration, IEmployerProjectionAuditService auditService)
    {
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService)); 
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
    }

    public async Task<bool> Allow(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
    {
        Logger.LogDebug($"Now checking if projections can be generated for levy declaration events: {levySchemeDeclaration.ToDebugJson()}");
        if (levySchemeDeclaration.PayrollMonth == null)
            throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");
        if (!ApplicationConfiguration.AllowTriggerProjections)
        {
            Logger.LogWarning("Triggering of projections is disabled.");
            return false;
        }


        var levyPeriod = await Repository.Get(levySchemeDeclaration.AccountId, levySchemeDeclaration.PayrollYear,
            levySchemeDeclaration.PayrollMonth.Value);
        var lastReceivedTime = levyPeriod.GetLastTimeReceivedLevy();
        if (lastReceivedTime == null)
        {
            Logger.LogWarning($"No levy recorded for employer: {levySchemeDeclaration.AccountId}, period: {levySchemeDeclaration.PayrollYear}, {levySchemeDeclaration.PayrollMonth.Value}");
            return false;
        }

        var allowProjections = lastReceivedTime.Value.AddSeconds(SecondsToWaitToAllowProjections) <= DateTime.UtcNow;
        Logger.LogInformation($"Allow projections '{allowProjections}' for employer '{levySchemeDeclaration.AccountId}' in response to levy event.");

        if (!allowProjections)
        {
            return false;
        }

        if (!await _auditService.RecordRunOfProjections(levySchemeDeclaration.AccountId,nameof(ProjectionSource.LevyDeclaration)))
        {
            Logger.LogDebug($"Triggering of levy projections for employer {levySchemeDeclaration.AccountId} has already been started.");
            return false;
        }

        return true;
    }
}