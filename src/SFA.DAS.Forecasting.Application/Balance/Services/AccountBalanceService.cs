using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Domain.Balance.Services;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IAccountApiClient _accountApiClient;
        
        public AccountBalanceService(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public async Task<Models.Balance.BalanceModel> GetAccountBalance(long accountId)
        {
            var account = await _accountApiClient.GetAccount(accountId);
            
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