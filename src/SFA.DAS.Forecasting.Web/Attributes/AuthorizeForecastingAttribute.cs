using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Attributes
{
    public class AuthorizeForecastingAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return
                httpContext.Request.Url.IsLoopback || 
                base.AuthorizeCore(httpContext);
        }
    }
}