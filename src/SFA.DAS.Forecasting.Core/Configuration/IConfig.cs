using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Core.Configuration
{
	public interface IConfig
	{
		string StorageConnectionString { get; set; }
        string LevyDeclarationTableName { get; }
        string AllowedHashstringCharacters { get; set; }
        string Hashstring { get; set; }
        AccountApiConfiguration AccountApi { get; set; }
    }
}