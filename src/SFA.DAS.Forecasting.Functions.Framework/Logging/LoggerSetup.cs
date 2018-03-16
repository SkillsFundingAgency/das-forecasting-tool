using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using SFA.DAS.NLog.Logger;
using System;
using System.Configuration;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    public class LoggerSetup
    { 
        internal static NLogLogger Create(string functionPath, TraceWriter writer, Type type)
        {
            var appName = GetSetting("AppName");
            var localLogPath = GetSetting("NLog-Localdir");

            LogManager.Configuration = new LoggingConfiguration();
            LogManager.ThrowConfigExceptions = true;

            if (IsDevEnvironment)
                AddLocalTarget(localLogPath, appName);
            else
                AddRedisTarget(appName);

            AddAzureTarget(writer);
            return new NLogLogger(type);
        }

        private static void AddRedisTarget(string appName)
        {
            var target = new RedisTarget
            {
                Name = "RedisLog",
                AppName = appName,
                EnvironmentKey = "EnvironmentName",
                ConnectionStringKey = "LoggingRedisConnectionString",
                IncludeAllProperties = true,
                KeySettingsKey = "LoggingRedisKey",
                Layout = "${message}"

            };

            LogManager.Configuration.AddTarget(target);
            LogManager.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, "RedisLog");
        }

        private static void AddLocalTarget(string localLogPath, string appName)
        {
            var logFilePath = GetSetting("NLog-InternalLogFile");
            var layout = GetSetting("NLog-SimpleLayout");

            InternalLogger.LogFile = logFilePath.Replace("{APPNAME}", appName);

            var fileTarget = new FileTarget("Disk")
            {
                FileName = localLogPath + "/logs/${shortdate}/" + appName + ".${shortdate}.log",
                Layout = layout
            };

            LogManager.Configuration.AddTarget(fileTarget);
            LogManager.Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, "Disk");
        }

        private static void AddAzureTarget(TraceWriter writer)
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var azureTarget = new AzureFunctionLogTarget(writer);
            config.AddTarget("azure", azureTarget);

            azureTarget.Layout = @"${level:uppercase=true}|${threadid:padCharacter=0:padding=3}|${message}";

            var rule1 = new LoggingRule("*", LogLevel.Trace, azureTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }

        private static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static bool IsDevEnvironment =>
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEVELOPMENT") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);
    }
}
