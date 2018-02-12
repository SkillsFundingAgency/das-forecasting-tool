namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public interface IApplicationConfiguration
    {
        string DatabaseConnectionString { get; }
        string StorageConnectionString { get;  }
        string BackLink { get;  }
        string AllowedHashstringCharacters { get;  }
        string HashString { get;  }
        int SecondsToWaitToAllowAggregation { get; }
        int NumberOfMonthsToProject { get;  }
    }

    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string BackLink { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string HashString { get; set; }
        public int SecondsToWaitToAllowAggregation { get; set; }
        public int NumberOfMonthsToProject { get; set; }
    }
}
