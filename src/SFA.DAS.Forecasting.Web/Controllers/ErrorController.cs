using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Forecasting.Web.Configuration;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("accessdenied", Name = RouteNames.AccessDenied)]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;

            return View("AccessDenied");
        }

        [Route("notfound", Name = RouteNames.NotFound)]
        public ActionResult NotFound()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;

            return View("NotFound");
        }
    }
}