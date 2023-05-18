using System.Configuration;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ExternalUrlAction(this UrlHelper helper, string controllerName, string actionName = "", bool ignoreAccountId = false, string folderPath="")
        {

            var baseUrl = GetBaseUrl();
            if (string.IsNullOrWhiteSpace(controllerName))
                return baseUrl;

            var accountId = helper.ActionContext.RouteData.Values["hashedAccountId"];

            return ignoreAccountId ? $"{baseUrl}{controllerName}/{actionName}"
                                    : $"{baseUrl}{folderPath}accounts/{accountId}/{controllerName}/{actionName}";

        }

        private static string GetBaseUrl() //TODO FAI-625
        {
            return ConfigurationManager.AppSettings["MyaBaseUrl"].EndsWith("/")
                ? ConfigurationManager.AppSettings["MyaBaseUrl"]
                : ConfigurationManager.AppSettings["MyaBaseUrl"] + "/";
        }
    }
}