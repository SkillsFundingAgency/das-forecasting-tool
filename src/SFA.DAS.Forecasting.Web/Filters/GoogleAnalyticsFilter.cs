using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Filters
{
    public class GoogleAnalyticsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                var user = controller.User.Identity;
                var emailClaim = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(
                    claim => claim.Type == "email");
                string accountIdFromUrl = string.Empty;
                //if (user.HasClaim(c => c.Type.Equals(EmployerPsrsClaims.AccountsClaimsTypeIdentifier)))
                //{
                //    accountIdFromUrl =
                //        filterContext.RouteData.Values[RouteValues.EmployerAccountId]?.ToString().ToUpper();
                //}
                //controller.ViewBag.GaData = new GaData
                //{
                //    UserId = user.Claims.First(c => c.Type.Equals(EmployerPsrsClaims.IdamsUserIdClaimTypeIdentifier)).Value,
                //    Acc = accountIdFromUrl
                //};
            }

            base.OnActionExecuting(filterContext);
        }

        public class GaData
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Vpv { get; set; }
            public string Acc { get; set; }
        }
    }
}