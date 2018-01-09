using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.PaymentsAdapter.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class PublishPaymentEventFunction
    {
        [FunctionName("PublishPaymentEventFunction")]
        public static void Run(
            [QueueTrigger("payments-adapter-publish-payment")]PaymentMessage payment,
            TraceWriter traceWriter)
        {
            traceWriter.Info($"Now publishing payment: {payment.Id}, employer: {payment.EmployerAccountId}.");
        }
    }
}