using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Config;
using SFA.DAS.NLog.Logger;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    public class LoggerSetup
    {
        private static void HookNLogToAzureLog(TraceWriter writer)
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var azureTarget = new AzureFunctionLogTarget(writer);
            config.AddTarget("azure", azureTarget);

            azureTarget.Layout = @"${level:uppercase=true}|${threadid:padCharacter=0:padding=3}|${message}";

            var rule1 = new LoggingRule("*", LogLevel.Trace, azureTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }

        internal static NLogLogger Create(string functionPath, TraceWriter writer, Type type)
        {
            LogManager.ThrowConfigExceptions = true;
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(functionPath, "NLog.config"));

            if (IsDevEnvironment)
            {
                LogManager.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, "Disk");
            }

            HookNLogToAzureLog(writer);
            var logger = new NLogLogger(type);
            return logger;
        }

        public static bool IsDevEnvironment =>
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEV") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("DEVELOPMENT") ?? false) ||
            (ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false);
    }
}
