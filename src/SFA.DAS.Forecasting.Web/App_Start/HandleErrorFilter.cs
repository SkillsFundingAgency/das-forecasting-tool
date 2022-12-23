using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Rest;

namespace SFA.DAS.Forecasting.Web
{
    public class HandleErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is HttpOperationException ex && ex.Response.StatusCode == HttpStatusCode.Unauthorized)
            {
                filterContext.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
