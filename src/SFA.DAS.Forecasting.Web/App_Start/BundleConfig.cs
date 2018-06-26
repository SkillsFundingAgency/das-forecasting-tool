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
            bundles.Add(new StyleBundle("~/css/bundles/select2").Include("~/Content/dist/css/select2.min.css"));
            bundles.Add(new StyleBundle("~/css/bundles/screen").Include("~/Content/dist/css/screen.css"));

            bundles.Add(new StyleBundle("~/css/bundles/c3").Include("~/Content/dist/c3/c3.min.css"));

            bundles.Add(new StyleBundle("~/css/bundles/tippy").Include("~/Content/dist/css/tippy.css"));

            bundles.Add(new ScriptBundle("~/bundles/libs")
                .Include("~/Content/dist/js/jquery-1.10.2.min.js")
                .Include("~/Content/dist/js/stacker.js")
                .Include("~/Content/dist/js/showhide-content.js")
                .Include("~/Content/dist/js/select2.min.js")
                .Include("~/Content/dist/js/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/addApprenticeship")
                    .Include("~/Content/dist/js/apprenticeships.js")
                    .Include("~/Content/dist/js/add-apprenticeship-custom.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/c3")
                .Include("~/Content/dist/c3/d3.min.js")
                .Include("~/Content/dist/c3/c3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/screen")
                .Include("~/Content/dist/js/screen.js"));

            bundles.Add(new ScriptBundle("~/bundles/tippy").Include("~/Content/dist/js/tippy.all.min.js"));
        }
    }
}
