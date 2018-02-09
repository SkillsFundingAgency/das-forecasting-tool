using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public interface IApplicationConfiguration
    {
        string DatabaseConnectionString { get; set; }
        string StorageConnectionString { get; set; }
        string BackLink { get; set; }
        string AllowedHashstringCharacters { get; set; }
        string Hashstring { get; set; }
        int SecondsToWaitToAllowAggregation { get; set; }
        AccountApiConfiguration AccountApi { get; set; }
    }

    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string BackLink { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string Hashstring { get; set; }
        public int SecondsToWaitToAllowAggregation { get; set; }
        public AccountApiConfiguration AccountApi { get; set; }
    }
}
