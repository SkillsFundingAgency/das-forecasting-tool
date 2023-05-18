using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.ApprenticeshipCourses.Functions;
using SFA.DAS.Forecasting.ApprenticeshipCourses.Functions.StartupExtensions;
using SFA.DAS.Forecasting.Core.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();
            
        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration =  serviceProvider.GetService<IConfiguration>();
            
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
            .AddJsonFile("local.settings.json", true)
            .AddJsonFile("local.settings.Development.json", true)
#endif
            .AddEnvironmentVariables();

        if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                }
            );
        }

        var builtConfiguration = config.Build();
        builder.Services.AddConfiguration(builtConfiguration);
        
        builder.Services.AddServices();
        
        var forecastingJobsConfiguration = builtConfiguration
            .GetSection(nameof(ForecastingConnectionStrings))
            .Get<ForecastingConnectionStrings>();
        builder.Services.AddCosmosDbServices(forecastingJobsConfiguration.CosmosDbConnectionString);
        
        builder.Services.AddLogging();    
        builder.Services.AddApplicationInsightsTelemetryWorkerService();
        builder.Services.BuildServiceProvider();

    }
}