using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.PaymentsAdapter.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class GetPaymentDataFromPaymentsApiFunction
    {
        [FunctionName("GetPaymentDataFromPaymentsApiFunction")]
        public static async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            [Queue("payments-adapter-publish-payment")] ICollector<PaymentEvent> payments,
            TraceWriter traceWriter)
        {
            // ToDo: What do we do with the init round? About 2 million record currently.
            traceWriter.Info($"Getting payment data from payment api. Date: {DateTime.Now}");

            var url = Environment.GetEnvironmentVariable("PaymentsApiBaseUrl", EnvironmentVariableTarget.Process);
            
            var paymentEvents = new PaymentEvents(PaymentsConfig()); // User interface?

            //foreach (var payment in await paymentEvents.ReadAsync())
            //{
            //    payments.Add(payment);
            //}

            traceWriter.Info("Finished getting payment data from payment api.");
        }

        public static PaymentEventsApiConfig PaymentsConfig()
        {
            var url = Environment.GetEnvironmentVariable("PaymentsApiBaseUrl", EnvironmentVariableTarget.Process);
            var token = Environment.GetEnvironmentVariable("PaymentsApiToken", EnvironmentVariableTarget.Process);
            var conectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
            return new PaymentEventsApiConfig
            {
                ApiBaseUrl = url,
                ClientToken = token,
                StorageConnectionString = conectionString,
                MaxPages = 5
            };
        }
    }
}
