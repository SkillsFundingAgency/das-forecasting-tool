using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Application.Commitments.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Services;

namespace SFA.DAS.Forecasting.Commitments.Functions.StartupExtensions;

public static class AddServicesExtension
{
    public static void AddServices(this IServiceCollection builder)
    {
        builder.AddTransient<IApprovalsService, ApprovalsService>();
        builder.AddTransient<IPledgesService, PledgesService>();
        builder.AddTransient<IDeletePledgeApplicationCommitmentsHandler, DeletePledgeApplicationCommitmentsHandler>();
        builder.AddTransient<IStoreCommitmentHandler, StoreCommitmentHandler>();
        
        builder.AddHttpClient<IApiClient, ApiClient>();
        
        builder.AddTransient<IEmployerCommitmentRepository, EmployerCommitmentRepository>();
        builder.AddTransient<ICommitmentsDataService, CommitmentsDataService>();
        
        builder.AddTransient<IPaymentMapper, PaymentMapper>();
        builder.AddTransient<IQueueService, QueueService>();
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
        services.Configure<ForecastingConnectionStrings>(builtConfiguration.GetSection(nameof(ForecastingConnectionStrings)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ForecastingConnectionStrings>>().Value);
    }
}