using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class PaymentsEventsApiConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}