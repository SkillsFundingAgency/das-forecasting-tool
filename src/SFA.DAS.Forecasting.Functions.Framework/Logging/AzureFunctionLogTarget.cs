using System;
using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    [Target("AzureFunctionLog")]
    public sealed class AzureFunctionLogTarget : TargetWithLayout
    {
        public AzureFunctionLogTarget(TraceWriter azureLogTraceWriter)
        {
            AzureLogTraceWriter = azureLogTraceWriter;
        }

        [ThreadStatic] private TraceWriter _traceWriter;

        [RequiredParameter] public TraceWriter AzureLogTraceWriter
        {
            get => _traceWriter;
            set => _traceWriter = value;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = this.Layout.Render(logEvent);
            AzureLogTraceWriter.Info(logMessage);
        }
    }
}