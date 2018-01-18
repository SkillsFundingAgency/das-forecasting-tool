using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    [Target("AzureFunctionLog")]
    public sealed class AzureFunctionLogTarget : TargetWithLayout
    {
        public AzureFunctionLogTarget(TraceWriter azureLogTraceWriter)
        {
            AzureLogTraceWriter = azureLogTraceWriter;
        }

        [RequiredParameter]
        public TraceWriter AzureLogTraceWriter { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            AzureLogTraceWriter.Info(logMessage);
        }
    }
}