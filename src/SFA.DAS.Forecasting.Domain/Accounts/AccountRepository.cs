using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Accounts.Model;
using SFA.DAS.Forecasting.Domain.Accounts.Services;

namespace SFA.DAS.Forecasting.Domain.Accounts
{
    public interface IAccountRepository
    {
        Task<Account> Get(long employerAccountId);
        Task Store(Account account);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly IEmployerAccountDataService _dataService;

        public AccountRepository(IEmployerAccountDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }
        public async Task<Account> Get(long employerAccountId)
        {
            var employerAccount = await _dataService.Get(employerAccountId) ?? new EmployerAccount { EmployerAccountId = employerAccountId };
            return new Account(employerAccount);
        }

        public async Task Store(Account account)
        {
            await _dataService.Store(account._employerAccount);
        }
    }
}