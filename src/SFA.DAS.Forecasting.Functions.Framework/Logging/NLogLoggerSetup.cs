using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using SFA.DAS.NLog.Logger;
using System;
using System.IO;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    public class NLogLoggerSetup
    {
        internal static NLogLogger Create(Type type)
        {
            var appName = GetSetting("AppName");
            var localLogPath = GetSetting("LogDir");

            var config = new LoggingConfiguration();

            if (ConfigurationHelper.IsDevEnvironment)
                AddLocalTarget(config, localLogPath, appName);
            else
                AddRedisTarget(config, appName);

            LogManager.Configuration = config;
            LogManager.ThrowConfigExceptions = true;
            return new NLogLogger(type);
        }

        private static void AddRedisTarget(LoggingConfiguration config, string appName)
        {
            var target = new RedisTarget
            {
                Name = "RedisLog",
                AppName = appName,
                EnvironmentKey = GetSetting("EnvironmentName"),
                ConnectionStringKey = GetSetting("LoggingRedisConnectionString"),
                IncludeAllProperties = true,
                KeySettingsKey = GetSetting("LoggingRedisKey"),
                Layout = "${message}"
            };

            config.AddTarget(target);
            config.AddRule(GetMinLogLevel(), LogLevel.Fatal, "RedisLog");
        }

        private static void AddLocalTarget(LoggingConfiguration config, string localLogPath, string appName)
        {
            InternalLogger.LogFile = Path.Combine(localLogPath, $"{appName}\\nlog-internal.{appName}.log");
            var fileTarget = new FileTarget("Disk")
            {
                FileName = Path.Combine(localLogPath, $"{appName}\\{appName}.${{shortdate}}.log"),
                Layout = "${longdate} [${uppercase:${level}}] [${logger}] - ${message} ${onexception:${exception:format=tostring}}"
            };
            config.AddTarget(fileTarget);

            config.AddRule(GetMinLogLevel(), LogLevel.Fatal, "Disk");
        }

        private static string GetSetting(string key, bool isSensitive = false)
        {
            return ConfigurationHelper.GetAppSetting(key, isSensitive);
        }

        private static LogLevel GetMinLogLevel() => LogLevel.FromString(GetSetting("MinLogLevel") ?? "Info");
    }
}
