using SFA.DAS.HashingService;
using System;
using System.Linq;
using System.Web;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Web.Authentication
{
    public class MembershipService : IMembershipService
    {
        private static readonly string Key = typeof(MembershipContext).FullName;
        private readonly IMembershipProvider _membershipProvider;
        private HttpContextBase _httpContext;
        private IOwinWrapper _authenticationService;
        private IHashingService _hashingService;
        private readonly ILog _logger;

        public MembershipService(
            IMembershipProvider membershipProvider,
            HttpContextBase httpContext, 
            IOwinWrapper authenticationService, 
            IHashingService hashingService,
            ILog logger)
        {
            _membershipProvider = membershipProvider;
            _httpContext = httpContext;
            _authenticationService = authenticationService;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<MembershipContext> GetMembershipContext()
        {
            if (_httpContext.Items.Contains(Key))
            {
                return _httpContext.Items[Key] as MembershipContext;
            }

            if (!_authenticationService.IsUserAuthenticated())
            {
                _logger.Info("Unable to find memebership due to user is not authenticated");
                return null;
            }

            string userExternalIdClaimValue;
            if (!_authenticationService.TryGetClaimValue(Constants.UserExternalIdClaimKeyName, out userExternalIdClaimValue))
            {
                _logger.Info("Unable to find memebership due to external id not found");
                return null;
            }

            Guid userExternalId;
            if (!Guid.TryParse(userExternalIdClaimValue, out userExternalId))
            {
                _logger.Info("Unable to find memebership due to error parsing external user id");
                return null;
            }

            object accountHashedId;
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(Constants.AccountHashedIdRouteKeyName, out accountHashedId))
            {
                _logger.Info($"Unable to find memebership due to {nameof(Constants.AccountHashedIdRouteKeyName)} not found in RouteData");
                return null;
            }

            long accountId;
            if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out accountId))
            {
                _logger.Info($"Unable to find memebership due to hashing service not ablet to decode value {accountHashedId}");
                return null;
            }

            var memberships = (await _membershipProvider.GetMemberships(accountHashedId.ToString()));

            var membership = 
                memberships
                .FirstOrDefault(m => m.UserRef == userExternalId.ToString());

            if (membership == null)
            {
                _logger.Info($"Unable to find memebership due to {userExternalId} is missing from memberships, Total {memberships.Count()} memberships found");
                return null;
            }

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