using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public interface IApplicationConfiguration
    {
        string DatabaseConnectionString { get; }
        string StorageConnectionString { get;  }
        string BackLink { get;  }
        string AllowedHashstringCharacters { get;  }
        string Hashstring { get;  }
        int SecondsToWaitToAllowProjections { get; }
        int NumberOfMonthsToProject { get;  }
        bool LimitForecast { get; set; }
        string EmployerConnectionString { get; set; }
        ApiConfiguration ApiConfiguration { get; set; }
    }
}