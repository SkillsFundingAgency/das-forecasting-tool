using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Core.Configuration
{
	public class Config : IConfig
	{
		public string StorageConnectionString { get; set; }

        public string LevyDeclarationTableName { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string Hashstring { get; set; }
        public AccountApiConfiguration AccountApi { get; set; }
    }

    public class AccountApiConfiguration : IAccountApiConfiguration
    {
        public string ApiBaseUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string IdentifierUri { get; set; }

        public string Tenant { get; set; }
    }
}
