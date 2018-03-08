using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web
{
    public class HandleErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception as HttpResponseException;
            if (ex != null && ex.Response.StatusCode == HttpStatusCode.Unauthorized)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
