﻿using System.Configuration;
using Microsoft.Azure;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class DefaultRegistry: Registry
    {
        public DefaultRegistry()
        {
            ForSingletonOf<IApplicationConfiguration>().Use(ctx => new ApplicationConfiguration
            {
                StorageConnectionString = GetConnectionString("StorageConnectionString"),
                DatabaseConnectionString = GetConnectionString("ForecastingConnectionString"),
                EmployerConnectionString = GetConnectionString("EmployerConnectionString"),
                Hashstring = GetAppSetting("HashString"),
                AllowedHashstringCharacters = GetAppSetting("AllowedHashstringCharacters"),
                NumberOfMonthsToProject = int.Parse(GetAppSetting("NumberOfMonthsToProject") ?? "0"),
                SecondsToWaitToAllowProjections = int.Parse(GetAppSetting("SecondsToWaitToAllowProjections") ?? "0"),
                AccountApi = GetAccount(),
                PaymentEventsApi = new PaymentsEventsApiConfiguration
                {
                    ApiBaseUrl = GetAppSetting("PaymentsEvent-ApiBaseUrl"),
                    ClientToken = GetAppSetting("PaymentsEvent-ClientToken"),
                }
            });
        }

        private AccountApiConfiguration GetAccount()
        {
            return new AccountApiConfiguration
            {
                Tenant = CloudConfigurationManager.GetSetting("AccountApi-Tenant"),
                ClientId = CloudConfigurationManager.GetSetting("AccountApi-ClientId"),
                ClientSecret = CloudConfigurationManager.GetSetting("AccountApi-ClientSecret"),
                ApiBaseUrl = CloudConfigurationManager.GetSetting("AccountApi-ApiBaseUrl"),
                IdentifierUri = CloudConfigurationManager.GetSetting("AccountApi-IdentifierUri")
            };
        }

        public string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName];

        public string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            return connectionString?.ConnectionString ?? ConfigurationManager.AppSettings[name];
        }
    }
}