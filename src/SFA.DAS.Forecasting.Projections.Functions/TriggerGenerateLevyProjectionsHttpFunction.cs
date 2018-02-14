using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class TriggerGenerateLevyProjectionsHttpFunction
    {
        [FunctionName("TriggerGenerateLevyProjectionsHttpFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "TriggerGenerateLevyProjections/{employerAccountId}")]HttpRequestMessage req, long employerAccountId,
            [Queue(QueueNames.GenerateLevyProjections)]ICollector<GenerateLevyAccountProjection> messages,
            TraceWriter log)
        {
            messages.Add(new GenerateLevyAccountProjection
            {
                EmployerAccountId = employerAccountId
            } );

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
