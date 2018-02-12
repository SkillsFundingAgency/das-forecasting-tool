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
                DatabaseConnectionString = GetConnectionString("ForecastingConnectionString"),
                Hashstring = GetAppSetting("HashString"),
                AllowedHashstringCharacters = GetAppSetting("AllowedHashstringCharacters"),
                NumberOfMonthsToProject = int.Parse(GetAppSetting("NumberOfMonthsToProject") ?? "0"),
                SecondsToWaitToAllowProjections = int.Parse(GetAppSetting("SecondsToWaitToAllowProjections") ?? "0"),
            });
        }

        public string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName];

        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            return connectionString?.ConnectionString ?? ConfigurationManager.AppSettings[name];
        }
    }
}