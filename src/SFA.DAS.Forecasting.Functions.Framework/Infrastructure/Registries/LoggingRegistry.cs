using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using SFA.DAS.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NLog.Targets.Redis.DotNetCore;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure.Registries
{
    public class LoggingRegistry : Registry
    {
        public IConfiguration Configuration { get; }
        public LoggingRegistry()
        {
            For<ILoggingContext>().Use<ConsoleLoggingContext>();
            // For<ILog>().Use<NLogLogger>();
            For<ILog>().Use(NLogLoggerSetup.Create(null, null, new Dictionary<string, object> { { "app_Name", "das-forecasting-appc-f" } }));
            //For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<ILoggingContext>(), null)).AlwaysUnique();           

            //var nLogConfiguration = new NLogConfiguration();
            //nLogConfiguration.ConfigureNLog(Configuration);
        }

    }


    public class NLogConfiguration
    {
        public void ConfigureNLog(IConfiguration configuration)
        {
            var appName = GetSetting("AppName");
            var localLogPath = GetSetting("LogDir");

            var config = new LoggingConfiguration();

            if (ConfigurationHelper.IsDevEnvironment)
            {
                AddLocalTarget(config, localLogPath, appName);
            }
            else
            {
                AddRedisTarget(config, appName);
            }

            LogManager.Configuration = config;
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

        private static void AddRedisTarget(LoggingConfiguration config, string appName)
        {
            var target = new RedisTarget
            {
                Name = "RedisLog",
                AppName = appName,
                EnvironmentKeyName = "TEST",//"EnvironmentName",
                ConnectionStringName = "das-dev-log-rds.redis.cache.windows.net:6380,password=0J8kHTfmszL0hQHrRGeIQyius7WMlgdmSZhQFo0zWzc=,ssl=True,abortConnect=False", //"LoggingRedisConnectionString",
                IncludeAllProperties = true,
                Layout = "${message}"
            };

            config.AddTarget(target);
            config.AddRule(GetMinLogLevel(), LogLevel.Fatal, "RedisLog");
        }

        private static LogLevel GetMinLogLevel() => LogLevel.Info;

        private static string GetSetting(string key, bool isSensitive = false)
        {
            return ConfigurationHelper.GetAppSetting(key, isSensitive);
        }

    }
}