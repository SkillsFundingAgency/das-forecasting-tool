using SFA.DAS.Provider.Events.Api.Client.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class ForecastingConfiguration
    {
        public string EmployerRecruitBaseUrl { get; set; }
        public string ZenDeskSectionId { get; set; }
        public string ZenDeskSnippetKey { get; set; }
        public string ZenDeskCobrowsingSnippetKey { get; set; }
        public PaymentsEventsApiClientConfiguration PaymentsEventsApi { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
    }
}
