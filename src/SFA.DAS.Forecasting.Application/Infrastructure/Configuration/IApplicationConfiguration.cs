using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public interface IApplicationConfiguration: IApplicationConnectionStrings
    {
        string BackLink { get; }
        string AllowedHashStringCharacters { get; }
        string HashString { get; }
        int SecondsToWaitToAllowProjections { get; }
        int NumberOfMonthsToProject { get; }
        bool LimitForecast { get; set; }
        AccountApiConfiguration AccountApi { get; set; }
        PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
        IdentityServerConfiguration Identity { get; set; }
        string StubEmployerPaymentTable { get; set; }
        bool AllowTriggerProjections { get; }
        string ApprenticeshipsApiBaseUri { get; }
        string AppInsightsInstrumentationKey  { get; }
        bool FeatureExpiredFunds { get; set; }
    }
}