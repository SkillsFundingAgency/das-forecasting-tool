using FluentValidation.Mvc;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using SFA.DAS.NLog.Logger;
using System;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SFA.DAS.Forecasting.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog _logger;

        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("APPINSIGHTS_INSTRUMENTATIONKEY");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FluentValidationModelValidatorProvider.Configure(m => m.AddImplicitRequiredValidator = false);
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            _logger = DependencyResolver.Current.GetService<ILog>();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if(_logger == null)
                _logger = DependencyResolver.Current.GetService<ILog>();

            var exception = Server.GetLastError();

            _logger.Error(exception, $"Application_Error - {exception.Message}");
        }
    }
}