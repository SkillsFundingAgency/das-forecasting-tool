using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.PerformanceTests
{
    public static class Extensions
    {
        public static decimal GetTruncatedAmount(this decimal value)
        {
            return decimal.Round(value - (value % .01M), 2);
        }

        public static void SendPayment(this CloudQueue queue, PaymentCreatedMessage payment)
        {
            var payload = JsonConvert.SerializeObject(payment);
            queue.AddMessage(new CloudQueueMessage(payload));
        }

        public static bool ClearQueue(this CloudQueueClient cloudQueueClient, string queueName)
        {
            var queue = cloudQueueClient.GetQueueReference(queueName);
            if (!queue.Exists())
                return false;
            queue.Clear();
            return true;
        }
    }
}