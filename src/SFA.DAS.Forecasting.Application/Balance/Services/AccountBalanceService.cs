using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IHashingService _hashingService;

        public AccountBalanceService(IAccountApiClient accountApiClient,
            IHashingService hashingService)
        {
            _accountApiClient = accountApiClient;
            _hashingService = hashingService;
        }

        public async Task<Models.Balance.BalanceModel> GetAccountBalance(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);
            var account = await _accountApiClient.GetAccount(hashedAccountId);
            
            return new Models.Balance.BalanceModel
            {
                EmployerAccountId = accountId,
                RemainingTransferBalance = account.RemainingTransferAllowance,
                TransferAllowance = account.StartingTransferAllowance,
                Amount = account.Balance
            };
        }
    }
}