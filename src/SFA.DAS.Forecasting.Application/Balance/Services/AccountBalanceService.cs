using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IApiClient _apiClient;
        private readonly IEncodingService _encodingService;

        public AccountBalanceService(IApiClient apiClient, IEncodingService encodingService)
        {
            _apiClient = apiClient;
            _encodingService = encodingService;
        }

        public async Task<BalanceModel> GetAccountBalance(long accountId)
        {
            var account = await _apiClient.Get<GetAccountBalanceResponse>(new GetAccountBalanceRequest(_encodingService.Encode(accountId, EncodingType.AccountId)));
            
            return new BalanceModel
            {
                EmployerAccountId = accountId,
                RemainingTransferBalance = account.RemainingTransferAllowance,
                TransferAllowance = account.StartingTransferAllowance,
                Amount = account.Balance
            };
        }
    }
}