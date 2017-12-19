using System;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service
        [Route("signout")]
        public ActionResult SignOut()
        {
            throw new NotImplementedException();
            //return OwinWrapper.SignOutUser(Url.ExternalUrlAction("service", "signout", true));
        }
    }
}