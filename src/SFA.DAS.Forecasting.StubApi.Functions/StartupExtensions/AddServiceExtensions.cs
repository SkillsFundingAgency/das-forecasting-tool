using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Core.Configuration;
using AccountApiConfiguration = SFA.DAS.Forecasting.Core.Configuration.AccountApiConfiguration;

namespace SFA.DAS.Forecasting.StubApi.Functions.StartupExtensions;


public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IServiceCollection services, IConfigurationRoot builtConfiguration)
    {
        services.AddOptions();
        services.Configure<ForecastingJobsConfiguration>(builtConfiguration.GetSection(nameof(ForecastingJobsConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingJobsConfiguration>>().Value);
        
    }
}