using System;
using System.Configuration;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.AcceptanceTests.Infrastructure
{
    public class Config: IApplicationConnectionStrings
    {
        public TimeSpan TimeToWait => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"] ?? "00:00:30");
        public TimeSpan TimeToPause => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"] ?? "00:00:05");
        public string Environment => GetAppSetting("Environment");
        public string DatabaseConnectionString => GetConnectionString("DatabaseConnectionString");
        public string StorageConnectionString => GetConnectionString("StorageConnectionString");
        public string EmployerConnectionString => GetConnectionString("EmployerConnectionString");

        public bool IsDevEnvironment => (Environment?.Equals("DEVELOPMENT", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (Environment?.Equals("LOCAL", StringComparison.OrdinalIgnoreCase) ?? false);
        public string LevyFunctionUrl => GetAppSetting("LevyFunctionUrl");
        public string LevyPreLoadFunctionUrl => GetAppSetting("LevyPreLoadFunctionUrl");
        public string LevyPreLoadFunctionAllEmployersUrl => GetAppSetting("LevyPreLoadFunctionAllEmployersUrl");
        public string PaymentFunctionUrl => GetAppSetting("PaymentFunctionUrl");
        public string PaymentPreLoadHttpFunction => GetAppSetting("PaymentPreLoadHttpFunction");
        public string AllEmployersPaymentPreLoadHttpFunction => GetAppSetting("AllEmployersPaymentPreLoadHttpFunction");
        public string ProjectionPaymentFunctionUrl => GetAppSetting("ProjectionPaymentFunctionUrl");
        public string ProjectionLevyFunctionUrl => GetAppSetting("ProjectionLevyFunctionUrl");
        public int EmployerAccountId => int.Parse(GetAppSetting("EmployerAccountId"));
        public string HashedEmployerAccountId => GetAppSetting("HashedEmployerAccountId");
        public string AzureStorageConnectionString => GetConnectionString("StorageConnectionString");
        public string ApiInsertBalanceUrl => GetAppSetting("ApiInsertBalanceUrl");
        public string ApiInsertPaymentUrl => GetAppSetting("ApiInsertPaymentUrl");
        public string ApiInsertLevyUrl => GetAppSetting("ApiInsertLevyUrl");
        public string ApiInsertApprenticeshipsUrl => GetAppSetting("ApiInsertApprenticeshipsUrl");
        public string GetApprenticeshipHttpTriggerUrl => GetAppSetting("GetApprenticeshipHttpTriggerUrl");

        protected string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
        protected string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString ?? throw new InvalidOperationException($"{name} not found in connection strings.");

    }
}