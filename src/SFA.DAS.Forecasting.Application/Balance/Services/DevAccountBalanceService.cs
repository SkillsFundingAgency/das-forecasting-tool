using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class DevAccountBalanceService: IAccountBalanceService
    {
        private readonly IApplicationConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public DevAccountBalanceService(IApplicationConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = new HttpClient {BaseAddress = new Uri(_configuration.AccountApi.ApiBaseUrl)};
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<BalanceModel> GetAccountBalance(long accountId)
        {
            var uri = $"{_configuration.AccountApi.ApiBaseUrl}/api/accounts/{accountId}";
            
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error getting account balance using dev account api. Uri: {uri}, Status: {response.StatusCode}, Error: {response.Content}");
            var payload = await response.Content.ReadAsStringAsync();
            var account = JsonConvert.DeserializeObject<AccountDetailViewModel>(payload);
            return new BalanceModel
            {
                EmployerAccountId = account.AccountId,
                Amount = account.Balance,
                TransferAllowance = account.TransferAllowance,
                RemainingTransferBalance = account.TransferAllowance
            };
        }
    }
}