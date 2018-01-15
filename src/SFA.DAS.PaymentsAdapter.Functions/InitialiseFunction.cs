using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using SFA.DAS.Messaging.POC;

namespace SFA.DAS.PaymentsAdapter.Functions
{
    public static class InitialiseFunction
    {
        [FunctionName("InitialiseFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, 
            TraceWriter log)
        {
            var cloudQueueClient = Ioc.Container.GetInstance<CloudQueueClient>();
            log.Info($"Now creating queue: {QueueNames.PublishPaymentEvent}");
            await cloudQueueClient.GetQueueReference(QueueNames.PublishPaymentEvent).CreateIfNotExistsAsync();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
