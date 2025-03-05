using System;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Levy.Functions.StartupExtensions;

namespace SFA.DAS.Forecasting.Functions.UnitTests.FunctionInitialisation;

public class WhenResolvingLevyFunctionsDependencies
{
    [TestCase(typeof(IAllowAccountProjectionsHandler))]
    [TestCase(typeof(IStoreLevyDeclarationHandler))]
    [TestCase(typeof(IValidator<LevySchemeDeclarationUpdatedMessage>))]
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
        serviceCollection.AddCosmosDbServices(forecastingJobsConfiguration.CosmosDbConnectionString, false);
        serviceCollection.AddDatabaseRegistration(forecastingJobsConfiguration.DatabaseConnectionString, configuration["EnvironmentName"]);
    }
}