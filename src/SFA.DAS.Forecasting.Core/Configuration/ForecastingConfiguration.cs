namespace SFA.DAS.Forecasting.Core.Configuration
{
    public class ForecastingConfiguration
    {
        public string EmployerRecruitBaseUrl { get; set; }
        public string ZenDeskSectionId { get; set; }
        public string ZenDeskSnippetKey { get; set; }
        public string ZenDeskCobrowsingSnippetKey { get; set; }
        public PaymentsEventsApiConfiguration PaymentsEventsApi { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
        public double SecondsToWaitToAllowProjections { get; set; }
        public string StorageConnectionString { get; set; }
        public string DatabaseConnectionString { get; set; }
        public bool UseGovSignIn { get; set; }
        public string AllowedCharacters { get; set; }
        public string HashString { get; set; }
        public string CosmosDbConnectionString { get; set; }
        public IdentityServerConfiguration IdentityServerConfiguration { get; set; }
    }
}
