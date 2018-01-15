using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using SFA.DAS.Forecasting.Payments.Messages.Events;
using SFA.DAS.Messaging.POC;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public static class InitialiseFunction
    {
        [FunctionName("InitialiseFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info($"Adding subscription to message {typeof(PaymentEvent).FullName} to endpoint {QueueNames.ValidatePaymentEvent}");
            var subscriptionService = Ioc.Container.GetInstance<ISubscriptionService>();
            await subscriptionService.AddSubscription<PaymentEvent>(QueueNames.ValidatePaymentEvent);
            
            var cloudQueueClient = Ioc.Container.GetInstance<CloudQueueClient>();
            log.Info($"Now creating queue: {QueueNames.ValidatePaymentEvent}");
            await cloudQueueClient.GetQueueReference(QueueNames.ValidatePaymentEvent).CreateIfNotExistsAsync();
            log.Info($"Now creating queue: {QueueNames.StorePaymentEvent}");
            await cloudQueueClient.GetQueueReference(QueueNames.StorePaymentEvent).CreateIfNotExistsAsync();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
