using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.Orchestrators.Estimations;

namespace SFA.DAS.Forecasting.Web.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
        [TestCase(typeof(IEstimationOrchestrator))]
        [TestCase(typeof(IAddApprenticeshipOrchestrator))]
        public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
        {
            var configuration = GenerateConfiguration();
            var forecastingConfiguration = configuration
                .GetSection("ForecastingConfiguration")
                .Get<ForecastingConfiguration>();
            
            var hostEnvironment = new Mock<IWebHostEnvironment>();
            var serviceCollection = new ServiceCollection();

            
            serviceCollection.AddSingleton(hostEnvironment.Object);
            serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
            serviceCollection.AddConfigurationOptions(configuration);
            serviceCollection.AddDistributedMemoryCache();
            serviceCollection.AddApplicationServices(forecastingConfiguration);
            serviceCollection.AddDomainServices();
            serviceCollection.AddOrchestrators();
            serviceCollection.AddCosmosDbServices(forecastingConfiguration, false);
            
            serviceCollection.AddDatabaseRegistration(forecastingConfiguration, configuration["Environment"]);
            serviceCollection.AddLogging();
            
            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService(toResolve);
            Assert.IsNotNull(type);
        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ForecastingConfiguration:DatabaseConnectionString", "test"),
                    new KeyValuePair<string, string>("ForecastingConfiguration:AllowedCharacters", "ABCDEFGHJKLMN12345"),
                    new KeyValuePair<string, string>("ForecastingConfiguration:HashString", "ABC123"),
                    new KeyValuePair<string, string>("ForecastingConfiguration:CosmosDbConnectionString", "AccountEndpoint=https://localhost:8081;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;Database=Forecasting;Collection=ForecastingDev;ThroughputOffer=400"),
                    new KeyValuePair<string, string>("AccountApiConfiguration:ApiBaseUrl", "https://localhost:1"),
                    new KeyValuePair<string, string>("Environment", "test"),
                }
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
}