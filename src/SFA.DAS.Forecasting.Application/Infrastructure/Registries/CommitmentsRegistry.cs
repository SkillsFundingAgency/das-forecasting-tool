using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client.Http;
using SFA.DAS.Forecasting.Application.LocalDevRegistry;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using StructureMap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            if (ConfigurationHelper.ByPassMI)
            {
                For<ICommitmentsApiClient>().Use(c => c.GetInstance<ICommitmentsApiClientFactory>().CreateClient()).Singleton();
                For<ICommitmentsApiClientFactory>().Use<LocalDevCommitmentApiClientFactory>();
            }
            else
            {
                IncludeRegistry<CommitmentsApiClientRegistry2>();
            }
        }
    }

    public class CommitmentsApiClientRegistry2 : Registry
    {
        public CommitmentsApiClientRegistry2()
        {
            For<ICommitmentsApiClient>().Use(c => c.GetInstance<ICommitmentsApiClientFactory>().CreateClient()).Singleton();
            For<ICommitmentsApiClientFactory>().Use<CommitmentsApiClientFactory2>();
        }
    }

    public class CommitmentsApiClientFactory2 : ICommitmentsApiClientFactory
    {
        private readonly CommitmentsClientApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public CommitmentsApiClientFactory2(CommitmentsClientApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public ICommitmentsApiClient CreateClient()
        {
            var httpClientFactory = new ManagedIdentityHttpClientFactory2(_configuration, _loggerFactory);
            var httpClient = httpClientFactory.CreateHttpClient();
            var restHttpClient = new CommitmentsRestHttpClient(httpClient, _loggerFactory);
            var apiClient = new CommitmentsApiClient(restHttpClient);

            return apiClient;
        }
    }

    public class ManagedIdentityHttpClientFactory2 : IHttpClientFactory
    {
        private readonly IManagedIdentityClientConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public ManagedIdentityHttpClientFactory2(IManagedIdentityClientConfiguration configuration)
            : this(configuration, null)
        {
        }

        public ManagedIdentityHttpClientFactory2(IManagedIdentityClientConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClientBuilder = new HttpClientBuilder();

            if (_loggerFactory != null)
            {
                httpClientBuilder.WithLogging(_loggerFactory);
            }

            var httpClient = httpClientBuilder
                .WithDefaultHeaders()
                .WithManagedIdentityAuthorisationHeader(new ManagedIdentityTokenGenerator2(_configuration))
                .Build();

            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

            return httpClient;
        }
    }

    public class ManagedIdentityTokenGenerator2 : IManagedIdentityTokenGenerator
    {
        private readonly IManagedIdentityClientConfiguration _config;
        public ManagedIdentityTokenGenerator2(IManagedIdentityClientConfiguration config)
        {
            _config = config;
        }

        public Task<string> Generate()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var token = azureServiceTokenProvider.GetAccessTokenAsync(_config.IdentifierUri);

            return token;
        }
    }
}
