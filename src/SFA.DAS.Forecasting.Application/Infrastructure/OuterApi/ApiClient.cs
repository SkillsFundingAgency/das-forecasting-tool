using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IApplicationConfiguration _config;

        public ApiClient(HttpClient httpClient, IApplicationConfiguration config)
        {
            _httpClient = httpClient;
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(config.ApprenticeshipsApiBaseUri);    
            }
            
            _config = config;
        }

        public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
        {
            AddHeaders();

            var response = await _httpClient.GetAsync(request.GetUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }
        private void AddHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
            _httpClient.DefaultRequestHeaders.Remove("X-Version");
                
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.ApprenticeshipsApiSubscriptionKey);
            _httpClient.DefaultRequestHeaders.Add("X-Version", "1");
        }

        public string BaseUrl => _httpClient.BaseAddress.ToString();
    }
}