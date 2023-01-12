using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Handlers;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.ApprenticeshipCourses.Functions.StartupExtensions;
using SFA.DAS.Forecasting.Core.Configuration;

namespace SFA.DAS.Forecasting.Functions.UnitTests.FunctionInitialisation;

public class WhenResolvingApprenticeshipCoursesFunctionDependencies
{
    
    [TestCase(typeof(IStoreCourseHandler))]
    [TestCase(typeof(IGetCoursesHandler))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        
        var serviceCollection = new ServiceCollection();
        
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    private void SetupServiceCollection(ServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        var forecastingJobsConfiguration = configuration
            .GetSection("ForecastingJobsConfiguration")
            .Get<ForecastingJobsConfiguration>();
        serviceCollection.AddConfiguration(configuration);
        serviceCollection.AddServices();
        serviceCollection.AddCosmosDbServices(forecastingJobsConfiguration.CosmosDbConnectionString, false);
    }


    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("OuterApiConfiguration:OuterApiApiBaseUri", "https://localhost:1"),
                new KeyValuePair<string, string>("OuterApiConfiguration:OuterApiSubscriptionKey", "test"),
                new KeyValuePair<string, string>("ForecastingJobsConfiguration:CosmosDbConnectionString", "AccountEndpoint=https://localhost:8081;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;Database=Forecasting;Collection=ForecastingDev;ThroughputOffer=400"),
                new KeyValuePair<string, string>("Environment", "test"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}