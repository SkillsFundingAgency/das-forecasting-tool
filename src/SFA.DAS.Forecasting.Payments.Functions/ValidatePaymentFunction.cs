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
            if (payment.EarningDetails == null)
            {
                log.Error($"Invalid payment event. Does not contain earning details. Event: {JsonConvert.SerializeObject(payment)}");
                return null;
            }

            log.Info($"Finished validting payment. Data: {JsonConvert.SerializeObject(payment)}");
            return payment;
        }
    }
}
