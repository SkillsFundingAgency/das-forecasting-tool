using System;
using System.Configuration;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Mvc
{
    public class ForcastingRoutePrefix : RoutePrefixAttribute
    {
        public ForcastingRoutePrefix() : base()
        {
            
        }

        public ForcastingRoutePrefix(string prefix) : base(GetPrefix(prefix))
        {
            
        }

        private static string GetPrefix(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix), "Cannot be null");

            var commitmentsPrefix = ConfigurationManager.AppSettings["ForcastingRoutePrefix"];

            if (!string.IsNullOrWhiteSpace(commitmentsPrefix))
                return $"{commitmentsPrefix}/{prefix}";
            
            return prefix;
        }
    }
}