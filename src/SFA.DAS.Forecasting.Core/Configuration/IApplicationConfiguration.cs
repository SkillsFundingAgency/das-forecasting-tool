namespace SFA.DAS.Forecasting.Core.Configuration
{
    public interface IApplicationConfiguration : IApplicationConnectionStrings
    {
        string BackLink { get; }
        string AllowedHashStringCharacters { get; }
        string HashString { get; }
        int SecondsToWaitToAllowProjections { get; }
        int NumberOfMonthsToProject { get; }
        bool LimitForecast { get; set; }
        EAS.Account.Api.Client.AccountApiConfiguration AccountApi { get; set; }
        IdentityServerConfiguration Identity { get; set; }
        PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
        string StubEmployerPaymentTable { get; set; }
        bool AllowTriggerProjections { get; }
        string ApprenticeshipsApiBaseUri { get; }
        string AppInsightsInstrumentationKey { get; }
        bool FeatureExpiredFunds { get; set; }
        string ApprenticeshipsApiSubscriptionKey { get; set; }
    }
}