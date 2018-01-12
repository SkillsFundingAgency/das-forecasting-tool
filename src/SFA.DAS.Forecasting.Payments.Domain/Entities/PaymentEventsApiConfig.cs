using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.Forecasting.Payments.Domain.Entities
{
    public class PaymentEventsApiConfig : IPaymentsEventsApiConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string StorageConnectionString { get; set; }
        public int MaxPages { get; set; }
    }
}
