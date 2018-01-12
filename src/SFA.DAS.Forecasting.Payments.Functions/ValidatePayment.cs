using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public static class ValidatePayment
    {
        [FunctionName("ValidatePayment")]
        public static void Run([QueueTrigger("payments-adapter-publish-payment")]PaymentEvent payment,
            [Queue("valid-payments-items")] ICollector<PaymentEvent> payments,
            TraceWriter log)
        {
            log.Info($"Processing payment");

            // Validate

            payments.Add(payment);
        }
    }
}
