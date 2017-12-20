using System;
using System.Configuration;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Mvc
{
    public class ForecastingRoutePrefix : RoutePrefixAttribute
    {
        public ForecastingRoutePrefix() : base()
        {
            
        }

        public ForecastingRoutePrefix(string prefix) : base(GetPrefix(prefix))
        {
            
        }

        private static string GetPrefix(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix), "Cannot be null");

            var commitmentsPrefix = ConfigurationManager.AppSettings["ForecastingRoutePrefix"];

            if (!string.IsNullOrWhiteSpace(commitmentsPrefix))
                return $"{commitmentsPrefix}/{prefix}";
            
            return prefix;
        }
    }
}