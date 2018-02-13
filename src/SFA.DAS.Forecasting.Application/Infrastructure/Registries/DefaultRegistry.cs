using System.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
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

            ForSingletonOf<IHashingService>()
                .Use<HashingService.HashingService>()
                .Ctor<string>("allowedCharacters").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AllowedHashstringCharacters)
                .Ctor<string>("hashstring").Is(ctx => ctx.GetInstance<IApplicationConfiguration>().Hashstring);
            For<IAccountApiClient>().Use<AccountApiClient>()
                .Ctor<AccountApiConfiguration>().Is(ctx => ctx.GetInstance<IApplicationConfiguration>().AccountApi);
        }

        public string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName];

        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            return connectionString?.ConnectionString ?? ConfigurationManager.AppSettings[name];
        }
    }
}