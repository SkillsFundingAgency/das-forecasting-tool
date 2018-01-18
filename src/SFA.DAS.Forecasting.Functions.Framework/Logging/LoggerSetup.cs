using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using SFA.DAS.NLog.Logger;
using System;

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

        private static void UseFileLogging()
        {
            var targetName = "Disk";
            var config = LogManager.Configuration ?? new LoggingConfiguration();
            var fileTarget = new FileTarget(targetName);
            config.AddTarget(targetName, fileTarget);

            fileTarget.FileName = @"${var:localdir}/logs/${shortdate}/${var:appName}.${shortdate}.log";
            fileTarget.Layout = @"${var:simplelayout}";

            var rule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        public static NLogLogger Create(TraceWriter writer, Type type)
        {
            // ToDo: Move to config
            LogManager.Configuration = new LoggingConfiguration();
            LogManager.Configuration.Variables.Add("simplelayout", "${longdate} [${uppercase:${level}}] [${logger}] - ${message} ${onexception:${exception:format=tostring}} --&gt; ${all-event-properties}");
            LogManager.Configuration.Variables.Add("appName", "das-forecasting-levy-f");
            LogManager.Configuration.Variables.Add("localdir", "C:/temp/FAT");

            InternalLogger.LogLevel = LogLevel.Debug;
            InternalLogger.LogFile = "C:/Temp/FAT/forecasting.error.log";

            
            var logger = new NLogLogger(type, null, null);

            HookNLogToAzureLog(writer);
            UseFileLogging();

            return logger;
        }
    }
}
