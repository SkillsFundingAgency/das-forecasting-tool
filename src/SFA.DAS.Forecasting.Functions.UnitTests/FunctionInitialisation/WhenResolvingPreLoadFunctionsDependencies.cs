using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.PreLoad.Functions.StartupExtensions;

namespace SFA.DAS.Forecasting.Functions.UnitTests.FunctionInitialisation;

public class WhenResolvingPreLoadFunctionsDependencies
{
    [TestCase(typeof(IEmployerDatabaseService))]
    [TestCase(typeof(IPreLoadPaymentDataService))]
    [TestCase(typeof(IPaymentApiDataService))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Handlers(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    private static void SetupServiceCollection(ServiceCollection serviceCollection)
    {
        var configuration = ConfigurationTestHelper.GenerateConfiguration();
        var forecastingJobsConfiguration = configuration
            .GetSection("ForecastingConnectionStrings")
            .Get<ForecastingConnectionStrings>();
        serviceCollection.AddConfiguration(configuration);
        serviceCollection.AddServices();
        serviceCollection.AddLogging();
        serviceCollection.AddDatabaseRegistration(forecastingJobsConfiguration.DatabaseConnectionString, configuration["EnvironmentName"]);
    }
}