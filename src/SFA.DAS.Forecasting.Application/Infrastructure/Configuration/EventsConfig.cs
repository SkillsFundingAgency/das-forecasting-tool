using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Http;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class EventsConfig
    {
        public string NServiceBusEndpointName { get; set; }
        public bool NServiceBusUseDevTransport { get; set; }
        public string NServiceBusLicense { get; set; }
    }
}