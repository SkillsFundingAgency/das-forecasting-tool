using System.Web.Optimization;

namespace SFA.DAS.Forecasting.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screenie6").Include("~/Content/css/screen-ie6.css"));
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screenie7").Include("~/Content/css/screen-ie7.css"));
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screenie8").Include("~/Content/css/screen-ie8.css"));
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screen").Include("~/Content/css/screen.css"));

            bundles.Add(new StyleBundle("~/comt-assets/bundles/forecasting").Include("~/Content/css/forecasting.css"));
        }
    }
}
