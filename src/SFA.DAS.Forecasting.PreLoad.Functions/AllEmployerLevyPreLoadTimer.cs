using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployerLevyPreLoadTimer : IFunction
    {
        [FunctionName("AllEmployerLevyPreLoadTimer")]
        [return: Queue(QueueNames.PreLoadAllLevyRequest)]
        public static string Run([TimerTrigger("0 0 0 23 * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Verbose("Triggering scheduled run of refresh levy for employers.");
            return "RefreshLevyForEmployer";
        }
    }
}