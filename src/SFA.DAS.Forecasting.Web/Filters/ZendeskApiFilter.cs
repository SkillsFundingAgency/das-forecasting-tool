using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Web.Filters
{
    public class ZendeskApiFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                var user = controller.User;
                //EmployerIdentifier account = null;
                //if (user.HasClaim(c => c.Type.Equals(EmployerPsrsClaims.AccountsClaimsTypeIdentifier)))
                //{
                //    var accountIdFromUrl =
                //        filterContext.RouteData.Values[RouteValues.EmployerAccountId]?.ToString().ToUpper();
                //    var employerAccountClaim =
                //        user.FindFirst(c => c.Type.Equals(EmployerPsrsClaims.AccountsClaimsTypeIdentifier));
                //    var employerAccounts =
                //        JsonConvert.DeserializeObject<Dictionary<string, EmployerIdentifier>>(
                //            employerAccountClaim
                //                ?.Value);

                //    if (accountIdFromUrl != null) account = employerAccounts[accountIdFromUrl];
                //}
                //controller.ViewBag.ZendeskApiData = new ZendeskApiData
                //{
                //    UserId = user.Claims.First(c => c.Type.Equals(EmployerPsrsClaims.IdamsUserIdClaimTypeIdentifier)).Value,
                //    Name = user.Claims.First(c => c.Type.Equals(EmployerPsrsClaims.NameClaimsTypeIdentifier)).Value,
                //    Email = user.Claims.First(c => c.Type.Equals(EmployerPsrsClaims.EmailClaimsTypeIdentifier)).Value,
                //    Acc = account?.AccountId,
                //    Organization = account?.EmployerName
                //};
            }

            base.OnActionExecuting(filterContext);
        }
        
        public class ZendeskApiData
        {
            public string UserId { get; set; }
            public string Acc { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Organization { get; set; }
        }
    }
}