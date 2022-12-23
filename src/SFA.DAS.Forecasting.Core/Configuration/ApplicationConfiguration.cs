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
        public string ApprenticeshipsApiBaseUri { get; set; }
        public string AppInsightsInstrumentationKey { get; set; }
        public bool FeatureExpiredFunds { get; set; }
        public string ApprenticeshipsApiSubscriptionKey { get; set; }
    }
    
    public class IdentityServerConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string AuthorizeEndPoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string UserInfoEndpoint { get; set; }

        public bool UseCertificate { get; set; }
        public string Scopes { get; set; }
        public ClaimIdentifierConfiguration ClaimIdentifierConfiguration { get; set; }
        public string ChangePasswordLink { get; set; }
        public string ChangeEmailLink { get; set; }
        public string RegisterLink { get; set; }
        public string AccountActivationUrl { get; set; }
    }
    public class ClaimIdentifierConfiguration
     {
         public string ClaimsBaseUrl { get; set; }
         public string Id { get; set; }
         public string GivenName { get; set; }
         public string FamilyName { get; set; }
         public string Email { get; set; }
         public string DisplayName { get; set; }
     }
}