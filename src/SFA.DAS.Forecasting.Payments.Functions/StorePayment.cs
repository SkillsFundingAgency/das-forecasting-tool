using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public static class StorePayment
    {
        [FunctionName("StorePayment")]
        public static async Task Run([QueueTrigger("valid-payments-items")]PaymentEvent payment, TraceWriter log)
        {
            log.Info($"Storing payment to database");

            var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
            var paymentRepository = new PaymentsRepository(connectionString);
            await paymentRepository.StorePayment(payment);
        }
    }
}
