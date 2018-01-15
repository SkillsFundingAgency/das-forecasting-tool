using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    

    public static class ValidatePaymentFunction
    {
        [FunctionName("ValidatePaymentFunction")]
        [return: Queue(QueueNames.StorePaymentEvent)]
        public static PaymentEvent Run([QueueTrigger(QueueNames.ValidatePaymentEvent)]PaymentEvent payment,
            TraceWriter log)
        {
            log.Info($"Validating payment.");

            // Validate

            log.Info($"Finished validting payment. Data: {JsonConvert.SerializeObject(payment)}");
            return payment;
        }
    }
}
