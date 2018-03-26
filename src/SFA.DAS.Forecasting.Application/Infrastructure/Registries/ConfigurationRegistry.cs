using System;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            var config = GetConfiguration();
            ForSingletonOf<IApplicationConfiguration>().Use(config);
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
                AllowTriggerProjections = bool.Parse(ConfigurationHelper.GetAppSetting("AllowTriggerProjections", false) ?? "true")
            };

            SetApiConfiguration(configuration);
            return configuration;
        }

        private void SetApiConfiguration(ApplicationConfiguration config)
        {
            config.AccountApi = ConfigurationHelper.GetAccountApiConfiguration();
            config.PaymentEventsApi = ConfigurationHelper.GetPaymentsEventsApiConfiguration();
        }
    }
}
