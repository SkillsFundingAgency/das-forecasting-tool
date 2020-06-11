using System.Net.Http;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using StructureMap;
using PaymentsEventsApiConfiguration = SFA.DAS.Forecasting.Application.Infrastructure.Configuration.PaymentsEventsApiConfiguration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class PaymentsRegistry : Registry
    {
        public PaymentsRegistry()
        {
            For<PaymentsEventsApiConfiguration>().Use(c => c.GetInstance<ForecastingConfiguration>().PaymentsEventsApi).Singleton();
            For<IPaymentsEventsApiConfiguration>().Use(c => c.GetInstance<PaymentsEventsApiConfiguration>());

            if (ConfigurationHelper.IsDevOrAtEnvironment)
            {
                For<IPaymentsEventsApiClient>().Use<DevPaymentsEventsApiClient>();
            }
            else
            {
                For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>().Ctor<HttpClient>("httpClient").Is(c => CreateClient(c));
            }
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<ForecastingConfiguration>().PaymentsEventsApi;

            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
               ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
               : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config));

            return httpClientBuilder
                .WithDefaultHeaders()
                .Build();
        }
    }
}
