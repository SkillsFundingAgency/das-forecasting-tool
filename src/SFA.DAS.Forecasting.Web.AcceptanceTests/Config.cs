using System;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using Sfa.Automation.Framework.Enums;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    public class Config : IApplicationConnectionStrings
    {
        public TimeSpan TimeToWait => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"] ?? "00:00:30");
        public TimeSpan TimeToPause => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"] ?? "00:00:05");
        public string Environment => GetAppSetting("Environment");
        public string WebSiteUrl => GetAppSetting("WebSiteUrl");
        public string WebSiteUrlLocal => GetAppSetting("WebSiteUrlLocal");

        public string EmployerAccountID => GetAppSetting("EmployerAccountID");

        public WebDriver Browser => Enum.TryParse(GetAppSetting("Browser"), true, out WebDriver browser)
            ? browser
            : throw new InvalidOperationException($"Invalid browser: {GetAppSetting("Browser")}");

        public string DatabaseConnectionString => ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;

        public string StorageConnectionString => throw new NotImplementedException();

        public string EmployerConnectionString => throw new NotImplementedException();

        protected string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
    }
}