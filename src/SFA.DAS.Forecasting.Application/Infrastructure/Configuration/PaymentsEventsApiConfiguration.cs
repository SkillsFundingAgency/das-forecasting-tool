using SFA.DAS.Provider.Events.Api.Client.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class PaymentsEventsApiConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
    }
}