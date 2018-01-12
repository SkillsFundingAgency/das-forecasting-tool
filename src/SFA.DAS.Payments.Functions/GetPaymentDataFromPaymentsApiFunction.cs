using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SFA.DAS.Payments.Functions
{
    public static class GetPaymentDataFromPaymentsApiFunction
    {
        [FunctionName("GetPaymentDataFromPaymentsApiFunction")]
        public static SFA.DAS.Payments.Functions Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter traceWriter)
        {
            traceWriter.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            //TODO: replace with call to payments api
        }
    }
}
