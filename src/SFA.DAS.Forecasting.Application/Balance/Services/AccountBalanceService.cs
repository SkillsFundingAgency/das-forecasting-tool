using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class AccountBalanceService: IAccountBalanceService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IHashingService _hashingService;

        public AccountBalanceService(IAccountApiClient accountApiClient,
            IHashingService hashingService)
        {
            _accountApiClient = accountApiClient ?? throw new ArgumentNullException(nameof(accountApiClient));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
        }

        public async Task<decimal> GetAccountBalance(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);
            var account = await _accountApiClient.GetAccount(hashedAccountId);
            return account.Balance;
        }
    }
}