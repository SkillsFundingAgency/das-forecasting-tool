using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class ValidationFunction
    {
        [FunctionName("ValidationFunction")]
        public static void Run(
            [QueueTrigger(MessageEndpoints.ValidationFunction)]PaymentEvent payment, 
            TraceWriter log)
        {
            log.Info($"Validating payment: {payment}");
        }
    }
}
