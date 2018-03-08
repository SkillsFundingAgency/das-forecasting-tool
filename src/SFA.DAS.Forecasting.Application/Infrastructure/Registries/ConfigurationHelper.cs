using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public static class ConfigurationHelper
    {
        public static T GetConfiguration<T>(string serviceName)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            return configurationService.Get<T>();
        }
    }
}