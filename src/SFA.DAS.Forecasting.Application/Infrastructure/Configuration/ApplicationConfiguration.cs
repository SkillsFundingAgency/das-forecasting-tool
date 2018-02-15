using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Provider.Events.Api.Client;

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
        AccountApiConfiguration AccountApi { get; set; }
        string EmployerConnectionString { get; set; }

        PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
    }

    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string BackLink { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string Hashstring { get; set; }
        public int SecondsToWaitToAllowProjections { get; set; }
        public int NumberOfMonthsToProject { get; set; }
        public AccountApiConfiguration AccountApi { get; set; }
        public string EmployerConnectionString { get; set; }
        public PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
    }

    public class PaymentsEventsApiConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
