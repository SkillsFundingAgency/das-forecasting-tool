using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            var config = GetConfiguration();
            ForSingletonOf<IApplicationConfiguration>().Use(config);
        }

        private IApplicationConfiguration GetConfiguration()
        {
            var configuration = new ApplicationConfiguration
            {
                DatabaseConnectionString = GetConnectionString("DatabaseConnectionString"),
                StorageConnectionString = GetConnectionString("StorageConnectionString"),
                Hashstring = GetAppSetting("HashString"),
                AllowedHashstringCharacters = GetAppSetting("AllowedHashstringCharacters"),
                NumberOfMonthsToProject = int.Parse(GetAppSetting("NumberOfMonthsToProject") ?? "0"),
                SecondsToWaitToAllowProjections = int.Parse(GetAppSetting("SecondsToWaitToAllowProjections") ?? "0"),
                BackLink = GetAppSetting("BackLink")
            };
            return configuration;
        }



        private async Task<string> GetKeyValueSecret(string secretUri)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync(secretUri).ConfigureAwait(false);
            return secret.Value;
        }

        public string GetAppSetting(string keyName)
        {
            var value = ConfigurationManager.AppSettings[keyName];
            return string.IsNullOrEmpty(value) || (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
                   (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false)
                ? value
                : GetKeyValueSecret(value).Result;
        }

        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            return connectionString?.ConnectionString ?? ConfigurationManager.AppSettings[name];
        }
    }
}