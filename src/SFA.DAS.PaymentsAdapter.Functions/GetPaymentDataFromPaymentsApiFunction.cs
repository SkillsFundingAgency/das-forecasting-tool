using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.PaymentsAdapter.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class GetPaymentDataFromPaymentsApiFunction
    {
        [FunctionName("GetPaymentDataFromPaymentsApiFunction")]
        public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            [Queue("payments-adapter-publish-payment")] ICollector<Payment> payments,
            TraceWriter traceWriter)
        {
            traceWriter.Info($"Getting payment data from payment api. Date: {DateTime.Now}");
            //TODO: replace with call to payments api
            payments.Add(new Payment { Id = Guid.NewGuid().ToString("N"), EmployerAccountId = "12345"});
            payments.Add(new Payment { Id = Guid.NewGuid().ToString("N"), EmployerAccountId = "ABCDE" });
            traceWriter.Info("Finished getting payment data from payment api.");
        }
    }
}
