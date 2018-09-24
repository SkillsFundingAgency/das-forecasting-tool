using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ExternalUrlAction(this UrlHelper helper, string controllerName, string actionName = "", bool ignoreAccountId = false, string folderPath="")
        {

            var baseUrl = GetBaseUrl();
            if (controllerName.IsNullOrWhiteSpace())
                return baseUrl;

            var accountId = helper.RequestContext.RouteData.Values["hashedAccountId"];

            return ignoreAccountId ? $"{baseUrl}{controllerName}/{actionName}"
                                    : $"{baseUrl}{folderPath}accounts/{accountId}/{controllerName}/{actionName}";

        }

        private static string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["MyaBaseUrl"].EndsWith("/")
                ? ConfigurationManager.AppSettings["MyaBaseUrl"]
                : ConfigurationManager.AppSettings["MyaBaseUrl"] + "/";
        }
    }
}