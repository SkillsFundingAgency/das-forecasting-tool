using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Web.Filters;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetDisplayName(this ClaimsPrincipal user)
        {
            return user.FindFirst(EmployerRecruitClaims
                .IdamsUserDisplayNameClaimTypeIdentifier).Value;
        }

        public static string GetEmailAddress(this ClaimsPrincipal user)
        {
            return user.FindFirst(EmployerRecruitClaims
                .IdamsUserEmailClaimTypeIdentifier).Value;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(EmployerRecruitClaims
                .IdamsUserIdClaimTypeIdentifier).Value;
        }

        public static IEnumerable<string> GetEmployerAccounts(this ClaimsPrincipal user)
        {
            var employerAccountClaim = user.FindFirst(c =>
                c.Type.Equals(EmployerRecruitClaims.AccountsClaimsTypeIdentifier));

            if (string.IsNullOrEmpty(employerAccountClaim?.Value))
                return Enumerable.Empty<string>();

            var employerAccounts = JsonConvert.DeserializeObject<List<string>>(employerAccountClaim.Value);
            return employerAccounts;
        }
    }
}