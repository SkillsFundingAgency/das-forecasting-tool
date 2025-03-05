using System;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Payments.Functions.StartupExtensions;

namespace SFA.DAS.Forecasting.Functions.UnitTests.FunctionInitialisation;

public class WhenResolvingPaymentFunctionsDependencies
{
    [TestCase(typeof(IAllowAccountProjectionsHandler))]
    [TestCase(typeof(IProcessEmployerPaymentHandler))]
    [TestCase(typeof(IStoreCommitmentHandler))]
    [TestCase(typeof(IValidator<PaymentCreatedMessage>))]
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