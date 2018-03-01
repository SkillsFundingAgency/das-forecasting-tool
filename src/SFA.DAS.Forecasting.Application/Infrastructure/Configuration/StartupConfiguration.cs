using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
	public class StartupConfiguration : IStartupConfiguration
	{
		public string ServiceBusConnectionString { get; set; }
		public IdentityServerConfiguration Identity { get; set; }
		public string DashboardUrl { get; set; }
		public string DatabaseConnectionString { get; set; }
		public string Hashstring { get; set; }
		public string AllowedHashstringCharacters { get; set; }
		public string TokenCertificateThumbprint { get; set; }
		
		public StartupConfiguration()
		{
			DatabaseConnectionString = GetConnectionString("DatabaseConnectionString");
			ServiceBusConnectionString = GetConnectionString("ServiceBusConnectionString");
			DashboardUrl = GetAppSetting("DashboardUrl");
			Hashstring = GetAppSetting("HashString");
			AllowedHashstringCharacters = GetAppSetting("AllowedHashstringCharacters");
			Identity = new IdentityServerConfiguration
			{
				ClientId = GetAppSetting("ClientId"),
				ClientSecret = GetAppSetting("ClientSecret"),
				BaseAddress = GetAppSetting("BaseAddress"),
				AuthorizeEndPoint = GetAppSetting("AuthorizeEndPoint"),
				LogoutEndpoint = GetAppSetting("LogoutEndpoint"),
				TokenEndpoint = GetAppSetting("TokenEndpoint"),
				UserInfoEndpoint = GetAppSetting("UserInfoEndpoint"),
				UseCertificate = bool.Parse(GetAppSetting("UseCertificate")),
				Scopes = GetAppSetting("Scopes"),
				ChangePasswordLink = GetAppSetting("ChangePasswordLink"),
				ChangeEmailLink = GetAppSetting("ChangeEmailLink"),
				RegisterLink = GetAppSetting("RegisterLink"),
				AccountActivationUrl = GetAppSetting("AccountActivationUrl"),
				ClaimIdentifierConfiguration = new ClaimIdentifierConfiguration
				{
					ClaimsBaseUrl = GetAppSetting("ClaimsBaseUrl"),
					Id = GetAppSetting("Id"),
					GivenName = GetAppSetting("GivenName"),
					FamilyName = GetAppSetting("FamilyName"),
					Email = GetAppSetting("Email"),
					DisplayName = GetAppSetting("DisplayName"),
				}
			};
			TokenCertificateThumbprint = GetAppSetting("TokenCertificateThumbprint");
		}

		private async Task<string> GetKeyValueSecret(string secretUri)
		{
			var azureServiceTokenProvider = new AzureServiceTokenProvider();
			var keyVaultClient =
				new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
			var secret = await keyVaultClient.GetSecretAsync(secretUri).ConfigureAwait(false);
			return secret.Value;
		}

		public string GetAppSetting(string keyName)
		{
			var value = ConfigurationManager.AppSettings[keyName];
			return string.IsNullOrEmpty(value) || IsDevEnvironment
				? value
				: GetKeyValueSecret(value).Result;
		}

		public static bool IsDevEnvironment =>
			(ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
			(ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);

		public string GetConnectionString(string name)
		{
			var connectionString = ConfigurationManager.ConnectionStrings[name];
			return connectionString?.ConnectionString ?? ConfigurationManager.AppSettings[name];
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
