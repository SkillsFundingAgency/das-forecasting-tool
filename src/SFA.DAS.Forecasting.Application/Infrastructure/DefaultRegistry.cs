using System.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure
{
    public class DefaultRegistry: Registry
    {
        public DefaultRegistry()
        {
            ForSingletonOf<IApplicationConfiguration>().Use(ctx => new ApplicationConfiguration
            {
                StorageConnectionString = GetConnectionString("StorageConnectionString"),
                DatabaseConnectionString = GetConnectionString("ForecastingConnectionString")
            });
        }

        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            return connectionString?.ConnectionString ?? ConfigurationManager.AppSettings[name];
        }
    }
}