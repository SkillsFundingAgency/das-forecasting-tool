using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
