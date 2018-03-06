using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class StartupConfiguration : IStartupConfiguration
    {
        //public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string Hashstring { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string TokenCertificateThumbprint { get; set; }

        public StartupConfiguration()
        {
            DatabaseConnectionString = GetConnectionString("DatabaseConnectionString");
            //ServiceBusConnectionString = GetConnectionString("ServiceBusConnectionString");
            DashboardUrl = GetAppSetting("DashboardUrl", false);
            Hashstring = GetAppSetting("HashString", true);
            AllowedHashstringCharacters = GetAppSetting("AllowedHashstringCharacters", true);
            Identity = new IdentityServerConfiguration
            {
                ClientId = GetAppSetting("ClientId", false),
                ClientSecret = GetAppSetting("ClientSecret", true),
                BaseAddress = GetAppSetting("BaseAddress", false),
                AuthorizeEndPoint = GetAppSetting("AuthorizeEndPoint", false),
                LogoutEndpoint = GetAppSetting("LogoutEndpoint", false),
                TokenEndpoint = GetAppSetting("TokenEndpoint", false),
                UserInfoEndpoint = GetAppSetting("UserInfoEndpoint", false),
                UseCertificate = bool.Parse(GetAppSetting("UseCertificate", false)),
                Scopes = GetAppSetting("Scopes", false),
                ChangePasswordLink = GetAppSetting("ChangePasswordLink", false),
                ChangeEmailLink = GetAppSetting("ChangeEmailLink", false),
                RegisterLink = GetAppSetting("RegisterLink", false),
                AccountActivationUrl = GetAppSetting("AccountActivationUrl", false),
                ClaimIdentifierConfiguration = new ClaimIdentifierConfiguration
                {
                    ClaimsBaseUrl = GetAppSetting("ClaimsBaseUrl", false),
                    Id = GetAppSetting("Claim_Identifier_Id", false),
                    GivenName = GetAppSetting("Claim_Identifier_GivenName", false),
                    FamilyName = GetAppSetting("Claim_Identifier_FamilyName", false),
                    Email = GetAppSetting("Claim_Identifier_Email", false),
                    DisplayName = GetAppSetting("Claim_Identifier_DisplayName", false),
                }
            };
            TokenCertificateThumbprint = GetAppSetting("WEBSITE_LOAD_CERTIFICATES", false);
        }

        private string KeyVaultName => CloudConfigurationManager.GetSetting("KeyVaultName");
        private string KeyVaultBaseUrl => $"https://{CloudConfigurationManager.GetSetting("KeyVaultName")}.vault.azure.net";

        private async Task<string> GetSecret(string secretName)
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

        public string GetAppSetting(string keyName, bool isSensitive)
        {
            var value = ConfigurationManager.AppSettings[keyName];
            return IsDevEnvironment || !isSensitive
                ? value
                : GetSecret(keyName).Result;
        }

        public bool IsDevEnvironment =>
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEVELOPMENT") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);

        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                return GetAppSetting(name, true);

            return IsDevEnvironment
                ? connectionString
                : GetSecret(name).Result;
        }

    }

    public class IdentityServerConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string AuthorizeEndPoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string UserInfoEndpoint { get; set; }

        public bool UseCertificate { get; set; }
        public string Scopes { get; set; }
        public ClaimIdentifierConfiguration ClaimIdentifierConfiguration { get; set; }
        public string ChangePasswordLink { get; set; }
        public string ChangeEmailLink { get; set; }
        public string RegisterLink { get; set; }
        public string AccountActivationUrl { get; set; }
    }

    public class ClaimIdentifierConfiguration
    {
        public string ClaimsBaseUrl { get; set; }
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}
