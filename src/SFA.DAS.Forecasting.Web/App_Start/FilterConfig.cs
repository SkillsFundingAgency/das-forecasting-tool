using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Filters;

namespace SFA.DAS.Forecasting.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorFilter());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new ZendeskApiFilter());
        }
    }
}
