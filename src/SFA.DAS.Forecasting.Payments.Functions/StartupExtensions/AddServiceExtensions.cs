using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Application.Commitments.Services;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Payments.Validation;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Domain.Payments.Services;

namespace SFA.DAS.Forecasting.Payments.Functions.StartupExtensions;

public static class AddServiceExtensions
{
    public static void AddServices(this IServiceCollection builder)
    {
        builder.AddTransient<IAllowAccountProjectionsHandler, AllowAccountProjectionsHandler>();
        builder.AddTransient<IProcessEmployerPaymentHandler, ProcessEmployerPaymentHandler>();
        builder.AddTransient<IStoreCommitmentHandler, StoreCommitmentHandler>();
        builder.AddTransient<IValidator<PaymentCreatedMessage>, PaymentEventSuperficialValidator>();
        // builder.AddTransient<IPledgesService, PledgesService>();
        builder.AddTransient<IEmployerProjectionAuditService, EmployerProjectionAuditService>();
        // builder.AddTransient<IStoreCommitmentHandler, StoreCommitmentHandler>();
        //
        // builder.AddHttpClient<IApiClient, ApiClient>();
        //
        builder.AddTransient<IEmployerCommitmentRepository, EmployerCommitmentRepository>();
        builder.AddTransient<IEmployerCommitmentsRepository, EmployerCommitmentsRepository>();
        builder.AddTransient<IEmployerPaymentsRepository, EmployerPaymentsRepository>();
        builder.AddTransient<IEmployerPaymentRepository, EmployerPaymentRepository>();
        builder.AddTransient<IEmployerPaymentDataSession, EmployerPaymentDataSession>();
        
        builder.AddTransient<ICommitmentsDataService, CommitmentsDataService>();
        //
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
    }
}