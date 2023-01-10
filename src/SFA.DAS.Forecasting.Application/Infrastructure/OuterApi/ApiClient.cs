﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ForecastingConfiguration _config;

        public ApiClient(HttpClient httpClient, ForecastingConfiguration config)
        {
            _httpClient = httpClient;
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(config.OuterApiApiBaseUri);    
            }
            
            _config = config;
        }

        public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
            AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _config.OuterApiSubscriptionKey);
            httpRequestMessage.Headers.Add("X-Version", "1");
        }
    }
}