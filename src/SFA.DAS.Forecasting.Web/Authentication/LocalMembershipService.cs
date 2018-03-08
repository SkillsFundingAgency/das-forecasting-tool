using System;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.Forecasting.Web.Authentication
{
    public class LocalMembershipService : IMembershipService
    {
        private static readonly string Key = typeof(MembershipContext).FullName;
        private HttpContextBase _httpContext;
        private readonly IOwinWrapper _authenticationService;

        public LocalMembershipService(
            HttpContextBase httpContext,
            IOwinWrapper authenticationService)
        {
            _httpContext = httpContext;
            _authenticationService = authenticationService;
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

            object accountHashedId;
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(Constants.AccountHashedIdRouteKeyName, out accountHashedId))
            {
                return null;
            }

            Guid userExternalId;
            if (!Guid.TryParse(userExternalIdClaimValue, out userExternalId))
            {
                return null;
            }

            return await Task.FromResult(
                new MembershipContext
                {
                    HashedAccountId = accountHashedId.ToString(),
                    UserEmail = string.Empty,
                    UserRef = userExternalId.ToString()

                });
        }

        public async Task<bool> ValidateMembership()
        {
            var membership = await GetMembershipContext();

            return membership != null;
        }
    }
}