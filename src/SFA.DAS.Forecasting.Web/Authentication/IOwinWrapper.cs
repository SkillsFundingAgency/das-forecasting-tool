using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Authentication
{
	public interface IOwinWrapper
	{
		ActionResult SignOutUser(string redirectUrl);
		bool IsUserAuthenticated();
        bool TryGetClaimValue(string key, out string value);
    }
}
