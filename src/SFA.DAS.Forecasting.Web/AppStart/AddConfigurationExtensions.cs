using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Core.Configuration;
using AccountApiConfiguration = SFA.DAS.EAS.Account.Api.Client.AccountApiConfiguration;

namespace SFA.DAS.Forecasting.Web;

public static class AddConfigurationExtensions
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ForecastingConfiguration>(configuration.GetSection(nameof(ForecastingConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingConfiguration>>().Value);

        services.Configure<AccountApiConfiguration>(configuration.GetSection(nameof(AccountApiConfiguration)));

        services.AddSingleton<IAccountApiConfiguration, AccountApiConfiguration>();
    }
}