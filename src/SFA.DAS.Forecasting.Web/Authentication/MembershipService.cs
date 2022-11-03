using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Web.Authentication
{
    public class MembershipService : IMembershipService
    {
        private static readonly string Key = typeof(MembershipContext).FullName;
        private readonly IMembershipProvider _membershipProvider;
        private readonly HttpContextBase _httpContext;
        private readonly IOwinWrapper _authenticationService;
        private readonly IApplicationConfiguration _configuration;

        public MembershipService(
            IMembershipProvider membershipProvider,
            HttpContextBase httpContext,
            IOwinWrapper authenticationService,
            IApplicationConfiguration configuration)
        {
            _membershipProvider = membershipProvider;
            _httpContext = httpContext;
            _authenticationService = authenticationService;
            _configuration = configuration;
        }

        public async Task<MembershipContext> GetMembershipContext()
        {
            if (_httpContext.Items.Contains(Key))
            {
                return _httpContext.Items[Key] as MembershipContext;
            }

            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }
            
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(Constants.AccountHashedIdRouteKeyName, out var accountHashedId))
            {
                return null;
            }

            var userExternalId = string.Empty;
            var email = string.Empty;
            if (_configuration.UseGovSignIn)
            {
                if (!_authenticationService.TryGetClaimValue(ClaimTypes.NameIdentifier, out userExternalId))
                {
                    return null;
                }
                if (!_authenticationService.TryGetClaimValue(ClaimTypes.Email, out email))
                {
                    return null;
                }

                var accounts = (await _membershipProvider.GetUserAccounts(userExternalId, email).ConfigureAwait(false)).ToList();
                var account = accounts.FirstOrDefault(c => c.AccountId.Equals(accountHashedId.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (account == null)
                {
                    return null;
                }
                var govMembership = new MembershipContext
                {
                    HashedAccountId = accountHashedId.ToString(),
                    UserRef = userExternalId,
                    UserEmail = email
                };
                _httpContext.Items[Key] = govMembership;
                return govMembership;
            }

            if (!_authenticationService.TryGetClaimValue(Constants.UserExternalIdClaimKeyName, out userExternalId))
            {
                return null;
            }

            

            var memberships = (await _membershipProvider.GetMemberships(accountHashedId.ToString()).ConfigureAwait(false)).ToList();
            var membership = memberships.FirstOrDefault(m => m.UserRef.Equals(userExternalId, StringComparison.OrdinalIgnoreCase));
            if (membership == null)
            {
                return null;
            }

            _httpContext.Items[Key] = membership;
            return membership;
        }

        public async Task<bool> ValidateMembership()
        {
            var membership = await GetMembershipContext().ConfigureAwait(false);
            return membership != null;
        }
    }
}