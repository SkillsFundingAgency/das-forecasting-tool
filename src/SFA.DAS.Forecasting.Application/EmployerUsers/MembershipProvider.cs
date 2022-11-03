using System.Linq;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Client;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using System;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

namespace SFA.DAS.Forecasting.Application.EmployerUsers
{
    public interface IMembershipProvider
    {
        Task<IEnumerable<MembershipContext>> GetMemberships(string accountHashedId);
        Task<IEnumerable<EmployerUserAccounts>> GetUserAccounts(string userId, string email);
    }

    public class MembershipProvider : IMembershipProvider
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly ILog _logger;
        private readonly IApiClient _apiClient;

        public MembershipProvider(IAccountApiClient accountApiClient, ILog logger, IApiClient apiClient)
        {
            _accountApiClient = accountApiClient;
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<MembershipContext>> GetMemberships(string hashedAccountId)
        {
            try
            {
                var users = await _accountApiClient.GetAccountUsers(hashedAccountId).ConfigureAwait(false);
                return 
                    users
                    .Select(m => new MembershipContext
                    {
                        HashedAccountId = hashedAccountId,
                        UserEmail = m.Email,
                        UserRef = m.UserRef
                    });
            }
            catch(Exception ex)
            {
                _logger.Warn(ex, $"Unable to find users for account {hashedAccountId}");
                return new List<MembershipContext>();
            }
        }
        public async Task<IEnumerable<EmployerUserAccounts>> GetUserAccounts(string userId, string email)
        {
            try
            {
                var users = await _apiClient.Get<GetUserAccountsResponse>(new GetEmployerAccountRequest(userId, email)).ConfigureAwait(false);
                return
                    users.UserAccounts
                        .Select(m => new EmployerUserAccounts
                        {
                            AccountId = m.AccountId,
                            EmployerName = m.EmployerName,
                            Role = m.Role
                        });
            }
            catch (Exception)
            {
                return new List<EmployerUserAccounts>();
            }
        }
    }
}
