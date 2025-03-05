using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.RegistrationExtensions;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.AppStart;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Forecasting.Web.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(IEstimationOrchestrator))]
    [TestCase(typeof(IAddApprenticeshipOrchestrator))]
    [TestCase(typeof(IForecastingOrchestrator))]
    [TestCase(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    [Test]
    public void Then_Resolves_Authorization_Handlers()
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetServices(typeof(IAuthorizationHandler)).ToList();

        Assert.IsNotNull(type);
        type.Count.Should().Be(2);
        type.Should().ContainSingle(c => c.GetType() == typeof(EmployerAccountAuthorizationHandler));
    }

    private static void SetupServiceCollection(ServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        var forecastingConfiguration = configuration
            .GetSection("ForecastingConfiguration")
            .Get<ForecastingConfiguration>();
        
        var forecastingConnectionStrings = configuration
            .GetSection("ForecastingConnectionStrings")
            .Get<ForecastingConnectionStrings>();
        
        var hostEnvironment = new Mock<IWebHostEnvironment>();
        serviceCollection.AddSingleton(hostEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
        serviceCollection.AddConfigurationOptions(configuration);
        serviceCollection.AddDistributedMemoryCache();
        serviceCollection.AddAuthenticationServices();
        serviceCollection.AddApplicationServices(forecastingConfiguration);
        serviceCollection.AddDomainServices();
        serviceCollection.AddOrchestrators();
        serviceCollection.AddCosmosDbServices(forecastingConnectionStrings.CosmosDbConnectionString, false);

        serviceCollection.AddDatabaseRegistration(forecastingConnectionStrings.DatabaseConnectionString, configuration["EnvironmentName"]);
        serviceCollection.AddLogging();
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("ForecastingConnectionStrings:DatabaseConnectionString", "Server=tcp:test.server.net,1433;Initial Catalog=test-db;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"),
                new("ForecastingConfiguration:AllowedCharacters", "ABCDEFGHJKLMN12345"),
                new("ForecastingConfiguration:HashString", "ABC123"),
                new("ForecastingConnectionStrings:CosmosDbConnectionString", "AccountEndpoint=https://localhost:8081;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;Database=Forecasting;Collection=ForecastingDev;ThroughputOffer=400"),
                new("AccountApiConfiguration:ApiBaseUrl", "https://localhost:1"),
                new("OuterApiConfiguration:OuterApiApiBaseUri", "https://localhost:1"),
                new("OuterApiConfiguration:OuterApiSubscriptionKey", "test"),
                new("EnvironmentName", "test"),
                new("SFA.DAS.Encoding", "{'Encodings':[{'EncodingType':'AccountId','Salt':'test','MinHashLength':6,'Alphabet':'46789BCDFGHJKLMNPRSTVWXY'}]}")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}