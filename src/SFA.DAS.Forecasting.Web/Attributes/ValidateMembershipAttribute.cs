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
            if (filterContext.HttpContext.Request.Url.IsLoopback)
                return;

            var result = _membershipService().ValidateMembership();
            var taskIsComplete = Task.WaitAll(new[] { result }, 2 * 1000);

            if (!taskIsComplete)
            {
                _logger.Warn($"Task for {nameof(_membershipService)} not able to complete. ");
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            if (!result.Result)
            {
                _logger.Info("Not able to validate user");
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }
    }
}