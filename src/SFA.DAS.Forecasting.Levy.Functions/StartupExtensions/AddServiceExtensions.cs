using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Levy.Services;
using SFA.DAS.Forecasting.Application.Levy.Validation;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Shared;

namespace SFA.DAS.Forecasting.Levy.Functions.StartupExtensions;

public static class AddServicesExtension
{
    public static void AddServices(this IServiceCollection builder)
    {
        builder.AddTransient<IAllowAccountProjectionsHandler, AllowAccountProjectionsHandler>();
        builder.AddTransient<IStoreLevyDeclarationHandler, StoreLevyDeclarationHandler>();
        
        builder.AddScoped<IValidator<LevySchemeDeclarationUpdatedMessage>, LevyDeclarationEventValidator>();

        builder.AddTransient<ILevyDeclarationRepository, LevyDeclarationRepository>();
        builder.AddTransient<ILevyPeriodRepository, LevyPeriodRepository>();
        builder.AddTransient<ILevyDataSession, LevyDataSession>();
        builder.AddTransient<IPayrollDateService, PayrollDateService>();
        
        builder.AddTransient<IEmployerProjectionAuditService, EmployerProjectionAuditService>();

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