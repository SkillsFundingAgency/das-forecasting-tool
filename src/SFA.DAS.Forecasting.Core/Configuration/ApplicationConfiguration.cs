namespace SFA.DAS.Forecasting.Core.Configuration
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string BackLink { get; set; }
        public string AllowedHashStringCharacters { get; set; }
        public string HashString { get; set; }
        public int SecondsToWaitToAllowProjections { get; set; }
        public int NumberOfMonthsToProject { get; set; }
        public bool LimitForecast { get; set; }
        public string EmployerConnectionString { get; set; }
        public EAS.Account.Api.Client.AccountApiConfiguration AccountApi { get; set; }
        public PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string StubEmployerPaymentTable { get; set; }
        public bool AllowTriggerProjections { get; set; }
        public string OuterApiApiBaseUri { get; set; }
        public string AppInsightsInstrumentationKey { get; set; }
        public bool FeatureExpiredFunds { get; set; }
        public string ApprenticeshipsApiSubscriptionKey { get; set; }
    }
    
    public class IdentityServerConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string Scopes { get; set; }
    }
}