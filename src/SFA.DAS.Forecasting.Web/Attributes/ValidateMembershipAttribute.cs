using SFA.DAS.Forecasting.Web.Authentication;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Attributes
{
    public class ValidateMembershipAttribute : ActionFilterAttribute
    {
        private readonly Func<IMembershipService> _membershipService;

        public ValidateMembershipAttribute()
            : this(() => DependencyResolver.Current.GetService<IMembershipService>())
        {
        }

        public ValidateMembershipAttribute(Func<IMembershipService> membershipService)
        {
            _membershipService = membershipService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Url.IsLoopback)
                return;

            var result = _membershipService().ValidateMembership();
            var taskIsComplete = Task.WaitAll(new[] { result }, 2 * 1000);

            if (!taskIsComplete)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            if (!result.Result)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}