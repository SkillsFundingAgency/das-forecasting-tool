using System;
using System.Configuration;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.PerformanceTests.Infrastructure
{
    public class Config: IApplicationConnectionStrings
    {
        public static Config Instance = new Config();
        public TimeSpan TimeToWait => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"] ?? "00:00:30");
        public TimeSpan TimeToPause => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"] ?? "00:00:05");
        public string Environment => GetAppSetting("Environment");
        public string DatabaseConnectionString => GetConnectionString("DatabaseConnectionString");
        public string StorageConnectionString => GetConnectionString("StorageConnectionString");
        public string EmployerConnectionString => GetConnectionString("EmployerConnectionString");
        public string CosmosDbConnectionString => GetConnectionString("CosmosDBConnectionString");

        public bool IsDevEnvironment => (Environment?.Equals("DEVELOPMENT", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (Environment?.Equals("LOCAL", StringComparison.OrdinalIgnoreCase) ?? false);
        public int EmployerAccountId => int.Parse(GetAppSetting("EmployerAccountId"));
        public string HashedEmployerAccountId => GetAppSetting("HashedEmployerAccountId");
        public string AzureStorageConnectionString => GetConnectionString("StorageConnectionString");
        public string ApiInsertBalanceUrl => GetAppSetting("ApiInsertBalanceUrl");

        protected string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
        protected string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString;

    }
}