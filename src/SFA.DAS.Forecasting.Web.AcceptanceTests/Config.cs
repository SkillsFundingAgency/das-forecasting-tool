using System;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using Sfa.Automation.Framework.Enums;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    public class Config
    {
        public TimeSpan TimeToWait => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"] ?? "00:00:30");
        public TimeSpan TimeToPause => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"] ?? "00:00:05");
        public string Environment => GetAppSetting("Environment");
        public string WebSiteUrl => GetAppSetting("WebSiteUrl");

        public WebDriver Browser => Enum.TryParse(GetAppSetting("Browser"), true, out WebDriver browser)
            ? browser
            : throw new InvalidOperationException($"Invalid browser: {GetAppSetting("Browser")}");
        protected string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");

    }
}