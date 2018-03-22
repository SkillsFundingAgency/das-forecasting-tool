using Microsoft.Azure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class ApprenticeshipApiConfig
    {
        public string ApprenticeshipApiBaseUrl { get; set; }

        public string StandardsKey => "Standards";
        public string FrameworksKey => "Frameworks";

        public ApprenticeshipApiConfig()
        {
            // ToDo: Move toConfigurationHelpe when availiable
            ApprenticeshipApiBaseUrl = GetAppSetting("ApprenticeshipApiBaseUrl", false);
        }

        private string GetAppSetting(string keyName, bool isSensitive)
        {
            var value = ConfigurationManager.AppSettings[keyName];
            return IsDevEnvironment || !isSensitive
                ? value
                : GetSecret(keyName).Result;
        }

        private bool IsDevEnvironment =>
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEVELOPMENT") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);

        private async Task<string> GetSecret(string secretName)
        {
            string KeyVaultName = CloudConfigurationManager.GetSetting("KeyVaultName");
            string KeyVaultBaseUrl = $"https://{CloudConfigurationManager.GetSetting("KeyVaultName")}.vault.azure.net";

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
