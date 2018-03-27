using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityModel.Client;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using Microsoft.Owin;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Web.Authentication
{
	public class OwinWrapper : IOwinWrapper
	{
		private readonly IOwinContext _owinContext;
		private readonly IApplicationConfiguration _configuration;
	    private readonly HttpContext _httpContext;
		public OwinWrapper(IApplicationConfiguration configuration)
		{
			_configuration = configuration;
			_owinContext = HttpContext.Current.GetOwinContext();
		    _httpContext = HttpContext.Current;
		}

		public void SignInUser(string id, string displayName, string email)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, displayName),
				new Claim("email", email),
				new Claim("sub", id)
			};

			var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

			var authenticationManager = _owinContext.Authentication;
			authenticationManager.SignIn(claimsIdentity);
			_owinContext.Authentication.User = new ClaimsPrincipal(claimsIdentity);
		}

		public ActionResult SignOutUser(string redirectUrl)
		{
			var authenticationManager = _owinContext.Authentication;
			authenticationManager.SignOut("Cookies");

			return new RedirectResult(redirectUrl);
		}

		public string GetClaimValue(string claimKey)
		{
			var claimIdentity = ((ClaimsIdentity)_httpContext.User.Identity).Claims.FirstOrDefault(claim => claim.Type == claimKey);

			return claimIdentity == null ? "" : claimIdentity.Value;

		}

		public async Task UpdateClaims()
		{
            var constants = new Constants(_configuration.Identity);
			var userInfoEndpoint = constants.UserInfoEndpoint();
			var accessToken = GetClaimValue("access_token");

			var userInfoClient = new UserInfoClient(new Uri(userInfoEndpoint), accessToken);

			var userInfo = await userInfoClient.GetAsync();

			foreach (var ui in userInfo.Claims.ToList())
			{

				if (ui.Item1.Equals(DasClaimTypes.Email))
				{
					var emailClaim = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(
							claim => claim.Type == "email");
					var emailClaim2 = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(
							claim => claim.Type == DasClaimTypes.Email);
					((ClaimsIdentity)HttpContext.Current.User.Identity).RemoveClaim(emailClaim);
					((ClaimsIdentity)HttpContext.Current.User.Identity).RemoveClaim(emailClaim2);

					((ClaimsIdentity)HttpContext.Current.User.Identity).AddClaim(new Claim("email", ui.Item2));
					((ClaimsIdentity)HttpContext.Current.User.Identity).AddClaim(new Claim(DasClaimTypes.Email, ui.Item2));
				}
			}

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