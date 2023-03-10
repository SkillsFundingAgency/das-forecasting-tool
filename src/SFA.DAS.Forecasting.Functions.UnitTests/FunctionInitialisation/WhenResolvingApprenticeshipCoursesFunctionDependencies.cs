using System;
using Microsoft.Extensions.Configuration;
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
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Handlers(Type toResolve)
    {
        
        var serviceCollection = new ServiceCollection();
        
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    private void SetupServiceCollection(ServiceCollection serviceCollection)
    {
        var configuration = ConfigurationTestHelper.GenerateConfiguration();
        var forecastingJobsConfiguration = configuration
            .GetSection("ForecastingConnectionStrings")
            .Get<ForecastingConnectionStrings>();
        serviceCollection.AddConfiguration(configuration);
        serviceCollection.AddServices();
        serviceCollection.AddCosmosDbServices(forecastingJobsConfiguration.CosmosDbConnectionString, false);
    }
}