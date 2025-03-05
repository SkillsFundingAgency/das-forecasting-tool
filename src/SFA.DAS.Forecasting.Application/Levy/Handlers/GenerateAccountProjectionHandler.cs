using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Core;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Domain.Projections;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers;

public class GenerateAccountProjectionHandler
{
    private readonly IAccountProjectionRepository _accountProjectionRepository;
    private readonly ILogger<GenerateAccountProjectionHandler> _logger;
    private readonly ForecastingJobsConfiguration _applicationConfiguration;

    public GenerateAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, ILogger<GenerateAccountProjectionHandler> logger,
        ForecastingJobsConfiguration applicationConfiguration)
    {
        _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
    }

    public async Task Handle(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
    {
        _logger.LogDebug($"Now generating account projections for id: {levySchemeDeclaration.AccountId} in response to Levy event.");
        var accountProjections = await _accountProjectionRepository.InitialiseProjection(levySchemeDeclaration.AccountId);
        accountProjections.BuildLevyTriggeredProjections(DateTime.Today, ForecastingJobsConfiguration.NumberOfMonthsToProject, DateTime.UtcNow);
        foreach (var accountProjectionReadModel in accountProjections.Projections)
        {
            _logger.LogDebug($"Generated projection: {accountProjectionReadModel.ToJson()}");
        }
        await _accountProjectionRepository.Store(accountProjections);
        _logger.LogInformation($"Finished generating account projections for account: {levySchemeDeclaration.AccountId}.");
    }
}