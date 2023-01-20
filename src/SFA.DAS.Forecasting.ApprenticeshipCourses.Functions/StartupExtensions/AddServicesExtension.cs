using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Core.Configuration;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions.StartupExtensions;

public static class AddServicesExtension
{
    public static void AddServices(this IServiceCollection builder)//, IConfigurationRoot configuration)
    {
        builder.AddTransient<IStoreCourseHandler, StoreCourseHandler>();
        builder.AddTransient<IGetCoursesHandler, GetCoursesHandler>();
        builder.AddTransient<IStandardsService, StandardsService>();
        builder.AddTransient<IFrameworksService, FrameworksService>();
        
        builder.AddHttpClient<IApiClient, ApiClient>();
        
        builder.AddTransient<IApprenticeshipCourseDataService, ApprenticeshipCourseDataService>();
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
        
        //TODO this isnt a dependency we really want here
        var encodingConfigJson = builtConfiguration.GetSection("SFA.DAS.Encoding").Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);
    }
}