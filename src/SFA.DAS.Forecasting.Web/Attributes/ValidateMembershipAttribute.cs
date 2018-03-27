using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.NLog.Logger;
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
        private readonly ILog _logger;

        public ValidateMembershipAttribute()
            : this(() => DependencyResolver.Current.GetService<IMembershipService>())
        {
            _logger = DependencyResolver.Current.GetService<ILog>();
        }

        public ValidateMembershipAttribute(Func<IMembershipService> membershipService)
        {
            _membershipService = membershipService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Url?.IsLoopback ?? false)
                return;
            var task = _membershipService().ValidateMembership();
            task.Wait(TimeSpan.FromSeconds(5));
            if (task.Result) return;

            //if (!result.Wait(TimeSpan.FromSeconds(5)))
            //{
            //    _logger.Warn($"Task for {nameof(_membershipService)} not able to complete. ");
            //    throw new HttpResponseException(HttpStatusCode.InternalServerError);
            //}
            _logger.Warn("Not able to validate user");
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}