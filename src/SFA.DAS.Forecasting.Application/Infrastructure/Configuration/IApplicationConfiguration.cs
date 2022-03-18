﻿using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public interface IApplicationConfiguration : IApplicationConnectionStrings
    {
        string BackLink { get; }
        string AllowedHashStringCharacters { get; }
        string HashString { get; }
        int SecondsToWaitToAllowProjections { get; }
        int NumberOfMonthsToProject { get; }
        bool LimitForecast { get; set; }
        AccountApiConfiguration AccountApi { get; set; }
        CommitmentsClientApiConfiguration CommitmentsClientApiConfiguration { get; set; }
        IdentityServerConfiguration Identity { get; set; }
        PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
        string StubEmployerPaymentTable { get; set; }
        bool AllowTriggerProjections { get; }
        string ForecastingOuterApiBaseUri { get; }
        string AppInsightsInstrumentationKey { get; }
        bool FeatureExpiredFunds { get; set; }
        string ForecastingOuterApiSubscriptionKey { get; set; }
    }
}