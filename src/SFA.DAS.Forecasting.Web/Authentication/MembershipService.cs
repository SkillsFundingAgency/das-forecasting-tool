using SFA.DAS.HashingService;
using System;
using System.Linq;
using System.Web;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.Authentication
{
    public class MembershipService : IMembershipService
    {
        private static readonly string Key = typeof(MembershipContext).FullName;
        private readonly IMembershipProvider _membershipProvider;
        private HttpContextBase _httpContext;
        private IOwinWrapper _authenticationService;
        private IHashingService _hashingService;

        public MembershipService(
            IMembershipProvider membershipProvider,
            HttpContextBase httpContext, 
            IOwinWrapper authenticationService, 
            IHashingService hashingService)
        {
            _membershipProvider = membershipProvider;
            _httpContext = httpContext;
            _authenticationService = authenticationService;
            _hashingService = hashingService;
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

            string userExternalIdClaimValue;
            if (!_authenticationService.TryGetClaimValue(Constants.UserExternalIdClaimKeyName, out userExternalIdClaimValue))
            {
                return null;
            }

            Guid userExternalId;
            if (!Guid.TryParse(userExternalIdClaimValue, out userExternalId))
            {
                return null;
            }

            object accountHashedId;
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(Constants.AccountHashedIdRouteKeyName, out accountHashedId))
            {
                return null;
            }

            long accountId;
            if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out accountId))
            {
                return null;
            }

            var memberships = (await _membershipProvider.GetMemberships(accountHashedId.ToString()));

            var membership = 
                memberships
                .FirstOrDefault(m => m.UserRef == userExternalId.ToString());

            if (membership == null)
                return null;

            _httpContext.Items[Key] = membership;

            return membership;
        }

        public async Task<bool> ValidateMembership()
        {
            var membership = await GetMembershipContext();

            return membership != null;
        }
    }
}