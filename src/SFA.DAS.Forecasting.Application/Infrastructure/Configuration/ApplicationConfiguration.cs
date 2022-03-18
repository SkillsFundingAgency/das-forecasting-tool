using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
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
        public AccountApiConfiguration AccountApi { get; set; }
        public PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
        public CommitmentsClientApiConfiguration CommitmentsClientApiConfiguration { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string StubEmployerPaymentTable { get; set; }
        public bool AllowTriggerProjections { get; set; }
        public string ForecastingOuterApiBaseUri { get; set; }
        public string AppInsightsInstrumentationKey { get; set; }
        public bool FeatureExpiredFunds { get; set; }
        public string ForecastingOuterApiSubscriptionKey { get; set; }
    }
}