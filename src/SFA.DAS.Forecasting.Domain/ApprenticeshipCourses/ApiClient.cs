using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Domain.ApprenticeshipCourses
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ForecastingApi _config;

        public ApiClient(HttpClient httpClient, IOptions<ForecastingApi> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
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
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.Key);
            _httpClient.DefaultRequestHeaders.Add("X-Version", "1");
        }
    }
}