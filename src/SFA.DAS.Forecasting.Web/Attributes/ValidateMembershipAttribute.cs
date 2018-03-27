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
            var membershipService = _membershipService();
            var task = Task.Run(async () => await membershipService.ValidateMembership().ConfigureAwait(false));

            _logger.Info($"Is a member of account: {task.Result}");
            if (task.Result) return;
            _logger.Warn("Not able to validate user");
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}