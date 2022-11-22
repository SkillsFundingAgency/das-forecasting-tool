using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;

namespace SFA.DAS.Forecasting.Web.Authentication
{
	public class OwinWrapper : IOwinWrapper
	{
		private readonly IOwinContext _owinContext;

        public OwinWrapper()
		{
            _owinContext = HttpContext.Current.GetOwinContext();
        }


		public ActionResult SignOutUser(string redirectUrl)
		{
			var authenticationManager = _owinContext.Authentication;
			authenticationManager.SignOut("Cookies");

			return new RedirectResult(redirectUrl);
		}

        public bool IsUserAuthenticated()
        {
            return _owinContext.Authentication.User.Identity.IsAuthenticated;
        }

        public bool TryGetClaimValue(string key, out string value)
        {
            var identity = _owinContext.Authentication.User.Identity as ClaimsIdentity;
            var claim = identity?.Claims.FirstOrDefault(c => c.Type == key);

            value = claim?.Value;

            return value != null;
        }
    }
}