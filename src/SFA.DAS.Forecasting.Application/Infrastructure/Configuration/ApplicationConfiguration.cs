namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public interface IApplicationConfiguration
    {
        string DatabaseConnectionString { get; set; }
        string StorageConnectionString { get; set; }
        string BackLink { get; set; }
        string AllowedHashstringCharacters { get; set; }
        string HashString { get; set; }
        int SecondsToWaitToAllowAggregation { get; set; }
    }

    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string BackLink { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string HashString { get; set; }
        public int SecondsToWaitToAllowAggregation { get; set; }
    }
}
