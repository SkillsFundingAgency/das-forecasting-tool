using System;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Provider.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            var config = GetConfiguration();
            ForSingletonOf<IApplicationConfiguration>().Use(config);
            ForSingletonOf<IApplicationConnectionStrings>().Use(config);
            ForSingletonOf<IAccountApiConfiguration>().Use(config.AccountApi);
            ForSingletonOf<IPaymentsEventsApiConfiguration>().Use(config.PaymentEventsApi);
        }

        private IApplicationConfiguration GetConfiguration()
        {
            var configuration = new ApplicationConfiguration
            {
                DatabaseConnectionString = ConfigurationHelper.GetConnectionString("DatabaseConnectionString"),
                EmployerConnectionString = ConfigurationHelper.GetConnectionString("EmployerConnectionString"),
                StorageConnectionString = ConfigurationHelper.GetConnectionString("StorageConnectionString"),

                Hashstring = ConfigurationHelper.GetAppSetting("HashString", true),
                AllowedHashstringCharacters = ConfigurationHelper.GetAppSetting("AllowedHashstringCharacters", true),
                NumberOfMonthsToProject = int.Parse(ConfigurationHelper.GetAppSetting("NumberOfMonthsToProject", false) ?? "0"),
                SecondsToWaitToAllowProjections = int.Parse(ConfigurationHelper.GetAppSetting("SecondsToWaitToAllowProjections", false) ?? "0"),
                BackLink = ConfigurationHelper.GetAppSetting("BackLink", false),
                LimitForecast = Boolean.Parse(ConfigurationHelper.GetAppSetting("LimitForecast", false) ?? "false"),
                StubEmployerPaymentTable = ConfigurationHelper.GetAppSetting("Stub-EmployerPaymentTable", false),
                AllowTriggerProjections = bool.Parse(ConfigurationHelper.GetAppSetting("AllowTriggerProjections", false) ?? "true"),
                ApprenticeshipsApiBaseUri = ConfigurationHelper.GetAppSetting("ApprenticeshipsApiBaseUri", false)
            };

            SetApiConfiguration(configuration);
            return configuration;
        }

        private void SetApiConfiguration(ApplicationConfiguration config)
        {
            config.AccountApi = ConfigurationHelper.GetAccountApiConfiguration();
            config.PaymentEventsApi = ConfigurationHelper.GetPaymentsEventsApiConfiguration();
            config.CommitmentsApi = ConfigurationHelper.GetCommitmentsApiConfiguration();
        }
    }
}
