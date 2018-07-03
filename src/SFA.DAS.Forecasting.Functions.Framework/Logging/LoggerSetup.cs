using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using SFA.DAS.NLog.Logger;
using System;
using System.Configuration;
using System.IO;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    public class LoggerSetup
    {
        internal static NLogLogger Create(ExecutionContext executionContext, TraceWriter writer, Type type)
        {
            var appName = GetSetting("AppName");
            var localLogPath = GetSetting("LogDir");

            LogManager.Configuration = new LoggingConfiguration();
            LogManager.ThrowConfigExceptions = true;

            if (ConfigurationHelper.IsDevOrAtEnvironment)
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
                EnvironmentKey = GetSetting("EnvironmentName"),
                ConnectionStringKey = GetSetting("LoggingRedisConnectionString"),
                IncludeAllProperties = true,
                KeySettingsKey = GetSetting("LoggingRedisKey"),
                Layout = "${message}"
            };

            LogManager.Configuration.AddTarget(target);
            LogManager.Configuration.AddRule(GetMinLogLevel(), LogLevel.Fatal, "RedisLog");
        }

        private static void AddLocalTarget(string localLogPath, string appName)
        {
            InternalLogger.LogFile = Path.Combine(localLogPath, $"{appName}\\nlog-internal.{appName}.log");
            var fileTarget = new FileTarget("Disk")
            {
                FileName = Path.Combine(localLogPath, $"{appName}\\{appName}.${{shortdate}}.log"),
                Layout = "${longdate} [${uppercase:${level}}] [${logger}] - ${message} ${onexception:${exception:format=tostring}}"
            };

            LogManager.Configuration.AddTarget(fileTarget);
            LogManager.Configuration.AddRule(GetMinLogLevel(), LogLevel.Fatal, "Disk");
        }

        private static void AddAzureTarget(TraceWriter writer)
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var azureTarget = new AzureFunctionLogTarget(writer);
            config.AddTarget("azure", azureTarget);

            azureTarget.Layout = @"${level:uppercase=true}|${threadid:padCharacter=0:padding=3}|${message}";

            var rule1 = new LoggingRule("*", GetMinLogLevel(), azureTarget);
            config.LoggingRules.Add(rule1);
            LogManager.Configuration = config;
        }

        private static string GetSetting(string key, bool isSensitive = false)
        {
            return ConfigurationHelper.GetAppSetting(key, isSensitive);
        }

        private static LogLevel GetMinLogLevel() => LogLevel.FromString(GetSetting("MinLogLevel") ?? "Info");
    }
}
