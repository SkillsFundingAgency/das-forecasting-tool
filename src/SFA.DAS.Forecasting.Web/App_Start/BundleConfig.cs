using System.Web.Optimization;

namespace SFA.DAS.Forecasting.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css/bundles/screenie6").Include("~/Content/dist/css/screen-ie6.css"));
            bundles.Add(new StyleBundle("~/css/bundles/screenie7").Include("~/Content/dist/css/screen-ie7.css"));
            bundles.Add(new StyleBundle("~/css/bundles/screenie8").Include("~/Content/dist/css/screen-ie8.css"));
            bundles.Add(new StyleBundle("~/css/bundles/screen").Include("~/Content/dist/css/screen.css"));
        }
    }
}
