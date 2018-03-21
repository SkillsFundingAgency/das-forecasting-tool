using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;

namespace SFA.DAS.Forecasting.Web.Authentication
{
	public interface IOwinWrapper
	{
		void SignInUser(string id, string displayName, string email);
		ActionResult SignOutUser(string redirectUrl);
		string GetClaimValue(string claimKey);
		Task UpdateClaims();
        bool IsUserAuthenticated();
        bool TryGetClaimValue(string key, out string value);
    }
}
