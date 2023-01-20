using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Commitments.Services;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Application.Levy.Services;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Domain.Projections.Services;

namespace SFA.DAS.Forecasting.Projections.Functions.StartupExtensions;

public static class AddServiceExtensions
{
    public static void AddServices(this IServiceCollection builder)
    {
        builder.AddTransient<IBuildAccountProjectionHandler, BuildAccountProjectionHandler>();
        builder.AddTransient<IGetAccountBalanceHandler, GetAccountBalanceHandler>();
        
        builder.AddTransient<IAccountProjectionRepository, AccountProjectionRepository>();
        builder.AddTransient<ILevyDataSession, LevyDataSession>();
        builder.AddTransient<IAccountProjectionDataSession, AccountProjectionDataSession>();
        builder.AddTransient<IAccountProjectionService, AccountProjectionService>();
        builder.AddTransient<ICurrentBalanceRepository, CurrentBalanceRepository>();
        builder.AddTransient<IBalanceDataService, BalanceDataService>();
        builder.AddTransient<IAccountBalanceService, AccountBalanceService>();
        builder.AddTransient<IEmployerCommitmentsRepository, EmployerCommitmentsRepository>();
        builder.AddTransient<ICommitmentsDataService, CommitmentsDataService>();
        builder.AddTransient<IExpiredFundsService, ExpiredFundsService>();
        builder.AddTransient<IExpiredFunds, ExpiredFunds>();
        builder.AddTransient<IEmployerPaymentDataSession, EmployerPaymentDataSession>();
        
        builder.AddHttpClient<IApiClient, ApiClient>();

        builder.AddTransient<IEncodingService, EncodingService>();
    }
}

public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IServiceCollection services, IConfigurationRoot builtConfiguration)
    {
        services.AddOptions();
        services.Configure<ForecastingJobsConfiguration>(builtConfiguration.GetSection(nameof(ForecastingJobsConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingJobsConfiguration>>().Value);
        
        var encodingConfigJson = builtConfiguration.GetSection("SFA.DAS.Encoding").Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);
    }
}