using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace SFA.DAS.Forecasting.Functions.UnitTests.FunctionInitialisation;

public static class ConfigurationTestHelper
{
    public static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("OuterApiConfiguration:OuterApiApiBaseUri", "https://localhost:1"),
                new("OuterApiConfiguration:OuterApiSubscriptionKey", "test"),
                new("ForecastingConnectionStrings:CosmosDbConnectionString", "AccountEndpoint=https://localhost:8081;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;Database=Forecasting;Collection=ForecastingDev;ThroughputOffer=400"),
                new("ForecastingConnectionStrings:ForecastingConnectionString", "test"),
                new("ForecastingJobsConfiguration:StorageConnectionString", "UseDevelopmentStorage=true;"),
                new("PaymentsEventsApiConfiguration:ApiBaseUrl", "https://localhost:2"),
                new("EnvironmentName", "test"),
                new("SFA.DAS.Encoding", "{'Encodings':[{'EncodingType':'AccountId','Salt':'test','MinHashLength':6,'Alphabet':'46789BCDFGHJKLMNPRSTVWXY'}]}")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}