using System;
using System.Configuration;

namespace SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries
{
    public class Config
    {
        public TimeSpan TimeToWait => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"] ?? "00:00:30");
        public TimeSpan TimeToPause => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"] ?? "00:00:05");
        public string Environment => GetAppSetting("Environment");
        public string FunctionBaseUrl => GetAppSetting("FunctionBaseUrl");

        public string AzureStorageConnectionString => GetAppSetting("AzureStorageConnectionString");
        public string LevyDeclarationsTable => GetAppSetting("LevyDeclarationsTable");
        public string EmployerPaymentsTable => GetAppSetting("EmployerPaymentsTable");

        protected string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
    }
}