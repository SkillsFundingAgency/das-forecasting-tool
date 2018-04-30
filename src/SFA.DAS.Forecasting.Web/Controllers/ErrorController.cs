using System.Net;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("accessdenied")]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View("AccessDenied");
        }

        [Route("notfound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View("NotFound");
        }
    }
}