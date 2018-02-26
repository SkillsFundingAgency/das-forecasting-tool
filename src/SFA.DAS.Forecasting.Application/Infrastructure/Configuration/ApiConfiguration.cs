using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class ApiConfiguration
    {
        public AccountApiConfiguration AccountApi { get; set; }

        public PaymentsEventsApiConfiguration PaymentEventsApi { get; set; }
    }
}
