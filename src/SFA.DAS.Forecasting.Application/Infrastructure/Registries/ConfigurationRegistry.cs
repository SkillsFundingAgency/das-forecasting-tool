using System;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Core;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class ConfigurationRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Forecasting";

        public ConfigurationRegistry()
        {
            IncludeRegistry<AutoConfigurationRegistry>();
            var config = GetConfiguration();
            ForSingletonOf<IApplicationConfiguration>().Use(config);
            ForSingletonOf<IApplicationConnectionStrings>().Use(config);
            ForSingletonOf<IAccountApiConfiguration>().Use(config.AccountApi);
            
            For<ForecastingConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ForecastingConfiguration>(ServiceName)).Singleton();
        }

        private IApplicationConfiguration GetConfiguration()
        {
            var configuration = new ApplicationConfiguration
            {
                DatabaseConnectionString = ConfigurationHelper.GetConnectionString("DatabaseConnectionString"),
                EmployerConnectionString = ConfigurationHelper.GetConnectionString("EmployerConnectionString"),
                StorageConnectionString = ConfigurationHelper.GetConnectionString("StorageConnectionString"),

                HashString = ConfigurationHelper.GetAppSetting("HashString", true),
                AllowedHashStringCharacters = ConfigurationHelper.GetAppSetting("AllowedHashStringCharacters", true),
                NumberOfMonthsToProject = int.Parse(ConfigurationHelper.GetAppSetting("NumberOfMonthsToProject", false) ?? "0"),
                SecondsToWaitToAllowProjections = int.Parse(ConfigurationHelper.GetAppSetting("SecondsToWaitToAllowProjections", false) ?? "0"),
                BackLink = ConfigurationHelper.GetAppSetting("BackLink", false),
                LimitForecast = Boolean.Parse(ConfigurationHelper.GetAppSetting("LimitForecast", false) ?? "false"),
                StubEmployerPaymentTable = ConfigurationHelper.GetAppSetting("Stub-EmployerPaymentTable", false),
                AllowTriggerProjections = bool.Parse(ConfigurationHelper.GetAppSetting("AllowTriggerProjections", false) ?? "true"),
                ApprenticeshipsApiBaseUri = ConfigurationHelper.GetAppSetting("ApprenticeshipsApiBaseUri", false),
                AppInsightsInstrumentationKey = ConfigurationHelper.GetAppSetting("APPINSIGHTS_INSTRUMENTATIONKEY", false),
                FeatureExpiredFunds = Boolean.Parse(ConfigurationHelper.GetAppSetting("FeatureExpiredFunds",false) ?? "true")
            };

            SetApiConfiguration(configuration);
            return configuration;
        }

        private void SetApiConfiguration(ApplicationConfiguration config)
        {
            config.AccountApi = ConfigurationHelper.GetAccountApiConfiguration();
            config.CommitmentsApi = ConfigurationHelper.GetCommitmentsApiConfiguration();
        }
    }
}
