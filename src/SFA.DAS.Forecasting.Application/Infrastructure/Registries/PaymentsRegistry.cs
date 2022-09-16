using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Client.DependencyResolution;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class PaymentsRegistry : PaymentsEventsApiClientRegistry
    {
        public PaymentsRegistry()
        {
            For<PaymentsEventsApiClientConfiguration>().Use(c => c.GetInstance<ForecastingConfiguration>().PaymentsEventsApi).Singleton();
            For<IPaymentsEventsApiClientConfiguration>().Use(c => c.GetInstance<PaymentsEventsApiClientConfiguration>());

            if (ConfigurationHelper.IsDevEnvironment)
            {
                For<IPaymentsEventsApiClient>().ClearAll().Use<DevPaymentsEventsApiClient>();
            }
        }
    }
}
