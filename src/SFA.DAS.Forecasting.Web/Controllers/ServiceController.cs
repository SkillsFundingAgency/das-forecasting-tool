using System;
using System.Web.Mvc;

using SFA.DAS.Forecasting.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ForcastingRoutePrefix("Service")]
    public class ServiceController : Controller
    {
        [Route("signout")]
        public ActionResult SignOut()
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            throw new NotImplementedException();
        }
    }
}