using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public static class ConfigurationHelper
    {
        private static string KeyVaultName => CloudConfigurationManager.GetSetting("KeyVaultName");
        private static string KeyVaultBaseUrl => $"https://{CloudConfigurationManager.GetSetting("KeyVaultName")}.vault.azure.net";

        public static AccountApiConfiguration GetAccountApiConfiguration()
        {
            return 
                IsDevOrTestEnvironment
                ? new AccountApiConfiguration
                {
                    Tenant = CloudConfigurationManager.GetSetting("AccountApi-Tenant"),
                    ClientId = CloudConfigurationManager.GetSetting("AccountApi-ClientId"),
                    ClientSecret = CloudConfigurationManager.GetSetting("AccountApi-ClientSecret"),
                    ApiBaseUrl = CloudConfigurationManager.GetSetting("AccountApi-ApiBaseUrl"),
                    IdentifierUri = CloudConfigurationManager.GetSetting("AccountApi-IdentifierUri")
                }
                : 
                GetConfiguration<AccountApiConfiguration>("SFA.DAS.EmployerAccountAPI");
        }

        public static PaymentsEventsApiConfiguration GetPaymentsEventsApiConfiguration()
        {
            return IsDevOrAtEnvironment
                ? new PaymentsEventsApiConfiguration
                {
                    ApiBaseUrl = CloudConfigurationManager.GetSetting("PaymentsEvent-ApiBaseUrl"),
                    ClientToken = CloudConfigurationManager.GetSetting("PaymentsEvent-ClientToken"),
                }
                : GetConfiguration<PaymentsEventsApiConfiguration>("SFA.DAS.PaymentsAPI");
        }

        public static CommitmentsApiConfig GetCommitmentsApiConfiguration()
        {
            return IsDevOrAtEnvironment
                ? new CommitmentsApiConfig
                {
                    BaseUrl = CloudConfigurationManager.GetSetting("Commitments-BaseUrl"),
                    ClientToken = CloudConfigurationManager.GetSetting("Commitments-ClientToken"),
                }
                : GetConfiguration<CommitmentsApiConfig>("SFA.DAS.CommitmentsAPI");
        }

        private static T GetConfiguration<T>(string serviceName)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = GetAppSetting("EnvironmentName", false);
            }

            var configurationRepository = new AzureTableStorageConfigurationRepository(GetAppSetting("ConfigurationStorageConnectionString", true));
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            return configurationService.Get<T>();
        }

        public static string GetAppSetting(string keyName, bool isSensitive)
        {
            var value = ConfigurationManager.AppSettings[keyName];
            return IsDevOrAtEnvironment || !isSensitive
                ? value
                : GetSecret(keyName).Result;
        }

        public static string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                return GetAppSetting(name, true);

            return IsDevOrAtEnvironment
                ? connectionString
                : GetSecret(name).Result;
        }

        public static bool IsDevEnvironment =>
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEVELOPMENT") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);

        public static bool IsDevOrAtEnvironment =>
            IsDevEnvironment || (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("AT") ?? false);

        public static bool IsDevOrTestEnvironment =>
            IsDevOrAtEnvironment || 
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("TEST") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("TEST2") ?? false);

        private static async Task<string> GetSecret(string secretName)
        {
            try
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var secret = await keyVaultClient.GetSecretAsync(KeyVaultBaseUrl, secretName).ConfigureAwait(false);
                return secret.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"There was an error getting the key vault secret. Secret name: {secretName}, KeyVault name: {KeyVaultName}, Error: {ex.Message}", ex);
            }
        }
    }
}