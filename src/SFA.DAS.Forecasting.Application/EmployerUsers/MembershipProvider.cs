using System.Linq;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Client;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Application.EmployerUsers
{
    public interface IMembershipProvider
    {
        Task<IEnumerable<MembershipContext>> GetMemberships(string accountHashedId);
    }

    public class MembershipProvider : IMembershipProvider
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly ILogger<MembershipProvider> _logger;

        public MembershipProvider(IAccountApiClient accountApiClient, ILogger<MembershipProvider> logger)
        {
            _accountApiClient = accountApiClient;
            _logger = logger;
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
                _logger.LogWarning(ex, $"Unable to find users for account {hashedAccountId}");
                return new List<MembershipContext>();
            }
        }
    }
}
