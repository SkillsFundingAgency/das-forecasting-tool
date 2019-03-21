using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployersPaymentPreLoadTimer : IFunction
    {
        [FunctionName("AllEmployersPaymentPreLoadTimer")]
        [return: Queue(QueueNames.PreLoadAllPaymentRequest)]
        public static string Run([TimerTrigger("0 0 0 13 * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Verbose("Triggering scheduled run of refresh payments for employers.");
            return "RefreshPaymentsForEmployer";
        }
    }
}