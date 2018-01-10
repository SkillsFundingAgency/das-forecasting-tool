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
    public static class Initialise
    {
        [FunctionName("Initialise")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, 
            TraceWriter traceWriter)
        {
            traceWriter.Info("Creating queues");
            var cloudQueueClient = Ioc.Container.GetInstance<CloudQueueClient>();
            await cloudQueueClient.GetQueueReference(MessageEndpoints.ValidationFunction).CreateIfNotExistsAsync();

            traceWriter.Info("Created queues, now adding subscriptions.");
            var subscriptionsService = Ioc.Container.GetInstance<ISubscriptionService>();
            await subscriptionsService.AddSubscription<PaymentEvent>(MessageEndpoints.ValidationFunction);
            return req.CreateResponse(HttpStatusCode.OK, "Success");
        }
    }
}
