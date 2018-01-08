using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.PaymentsAdapter.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class PublishPaymentEventFunction
    {
        [FunctionName("PublishPaymentEventFunction")]
        public static void Run(
            [QueueTrigger("payments-adapter-publish-payment")]Payment payment,
            TraceWriter traceWriter)
        {
            traceWriter.Info($"Now publishing payment: {payment.Id}, employer: {payment.EmployerAccountId}.");
        }
    }
}