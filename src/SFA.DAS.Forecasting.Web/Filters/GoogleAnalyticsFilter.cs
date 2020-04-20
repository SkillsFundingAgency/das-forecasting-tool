using System.Security.Claims;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Extensions;

namespace SFA.DAS.Forecasting.Web.Filters
{
    public class GoogleAnalyticsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                var user = (ClaimsPrincipal)controller.User;
                var userId = user?.GetUserId();
                controller.ViewBag.GaData = new GaData
                {
                    UserId = userId,
                    Acc = controller.RouteData.Values[RouteValues.EmployerAccountId]?.ToString().ToUpper()
                };
            }
            base.OnActionExecuting(filterContext);
        }

        public class GaData
        {
            public string UserId { get; set; }
            public string Vpv { get; set; }
            public string Acc { get; set; }
        }
    }
}