namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class ForecastingConfiguration
    {
        public string EmployerRecruitBaseUrl { get; set; }
        public string ZenDeskSectionId { get; set; }
        public string ZenDeskSnippetKey { get; set; }
        public string ZenDeskCobrowsingSnippetKey { get; set; }
        public PaymentsEventsApiConfiguration PaymentsEventsApi { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
    }
}
