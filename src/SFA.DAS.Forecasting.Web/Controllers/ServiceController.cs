using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Forecasting.Web.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [Route("accounts")]
    public class ServiceController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IStubAuthenticationService _stubAuthenticationService;

        public ServiceController(IConfiguration config, IStubAuthenticationService stubAuthenticationService)
        {
            _config = config;
            _stubAuthenticationService = stubAuthenticationService;
        }

        [Route("signout", Name = RouteNames.SignOut)]
        public async Task<IActionResult> SignOutEmployer()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token",idToken);
            return SignOut(
                authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

#if DEBUG
        [HttpGet]
        [Route("SignIn-Stub")]
        [AllowAnonymous]
        public IActionResult SigninStub()
        {
            return View("SigninStub", new List<string>{_config["StubId"],_config["StubEmail"]});
        }
        [HttpPost]
        [Route("SignIn-Stub")]
        [AllowAnonymous]
        public IActionResult SigninStubPost()
        {
            _stubAuthenticationService?.AddStubEmployerAuth(Response.Cookies, new StubAuthUserDetails
            {
                Email = _config["StubEmail"],
                Id = _config["StubId"]
            }, true);

            return RedirectToRoute("Signed-in-stub");
        }

        [Authorize]
        [HttpGet]
        [Route("signed-in-stub", Name = "Signed-in-stub")]
        public IActionResult SignedInStub()
        {
            return View();
        }
#endif
    }
}