using System.Web.Http;
using System.Web.Http.Controllers;

namespace SFA.DAS.Forecasting.Web.Attributes
{
    public class AuthorizeForecastingAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return actionContext.Request.RequestUri.IsLoopback || base.IsAuthorized(actionContext);
        }
    }
}