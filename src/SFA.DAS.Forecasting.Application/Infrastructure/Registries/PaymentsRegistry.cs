using System.Net.Http;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
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

            //if (ConfigurationHelper.IsDevOrAtEnvironment)
            //{
            //    For<IPaymentsEventsApiClient>().Use<DevPaymentsEventsApiClient>();
            //}
            //else
            //{
                For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
            //}
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<ForecastingConfiguration>().PaymentsEventsApi;

            HttpClient httpClient = new HttpClientBuilder()
                    .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config))
                    .WithHandler(new RequestIdMessageRequestHandler())
                    .WithHandler(new SessionIdMessageRequestHandler())
                    .WithDefaultHeaders()
                    .Build();


            return httpClient;
        }
    }
}
