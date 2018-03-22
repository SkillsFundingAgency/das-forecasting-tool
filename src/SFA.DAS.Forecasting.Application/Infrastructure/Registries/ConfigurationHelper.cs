using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public static class ConfigurationHelper
    {
        public static bool IsDevEnvironment =>
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEVELOPMENT") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);

        public static AccountApiConfiguration GetAccountApiConfiguration()
        {
            return IsDevEnvironment
                ? new AccountApiConfiguration
                {
                    Tenant = CloudConfigurationManager.GetSetting("AccountApi-Tenant"),
                    ClientId = CloudConfigurationManager.GetSetting("AccountApi-ClientId"),
                    ClientSecret = CloudConfigurationManager.GetSetting("AccountApi-ClientSecret"),
                    ApiBaseUrl = CloudConfigurationManager.GetSetting("AccountApi-ApiBaseUrl"),
                    IdentifierUri = CloudConfigurationManager.GetSetting("AccountApi-IdentifierUri")
                }
                : GetConfiguration<AccountApiConfiguration>("SFA.DAS.EmployerAccountAPI");
        }

        public static PaymentsEventsApiConfiguration GetPaymentsEventsApiConfiguration()
        {
            return IsDevEnvironment
                ? new PaymentsEventsApiConfiguration
                {
                    ApiBaseUrl = CloudConfigurationManager.GetSetting("PaymentsEvent-ApiBaseUrl"),
                    ClientToken = CloudConfigurationManager.GetSetting("PaymentsEvent-ClientToken"),
                }
                : GetConfiguration<PaymentsEventsApiConfiguration>("SFA.DAS.PaymentsAPI");
        }

        private static T GetConfiguration<T>(string serviceName)
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