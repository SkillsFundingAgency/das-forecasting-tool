using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using PaymentsEventsApiConfiguration = SFA.DAS.Provider.Events.Api.Client.Configuration.PaymentsEventsApiConfiguration;

namespace SFA.DAS.Forecasting.PreLoad.Functions.StartupExtensions;

public static class AddServiceExtensions
{
    public static void AddServices(this IServiceCollection builder)
    {
        builder.AddTransient<IEmployerDatabaseService, EmployerDatabaseService>();
        builder.AddTransient<IPreLoadPaymentDataService, PreLoadPaymentDataService>();
        builder.AddTransient<IPaymentApiDataService, PaymentApiDataService>();

        builder.AddHttpClient<IPaymentsEventsApiClient, PaymentsEventsApiClient>();
    }
}

public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IServiceCollection services, IConfigurationRoot builtConfiguration)
    {
        services.AddOptions();
        services.Configure<ForecastingJobsConfiguration>(builtConfiguration.GetSection(nameof(ForecastingJobsConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingJobsConfiguration>>().Value);
        services.Configure<OuterApiConfiguration>(builtConfiguration.GetSection(nameof(OuterApiConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<OuterApiConfiguration>>().Value);

        services.Configure<PaymentsEventsApiConfiguration>(builtConfiguration.GetSection(nameof(PaymentsEventsApiConfiguration)));
        services.AddSingleton<IPaymentsEventsApiConfiguration>(cfg =>
            builtConfiguration.GetSection(nameof(PaymentsEventsApiConfiguration))
                .Get<PaymentsEventsApiConfiguration>());
    }
}