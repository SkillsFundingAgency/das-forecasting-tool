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
            [Queue(QueueNames.GenerateProjections)]ICollector<GenerateAccountProjectionCommand> messages,
            TraceWriter log)
        {
            log.Verbose($"Received http request to generate projections for employer: {employerAccountId}");
            messages.Add(new GenerateAccountProjectionCommand
            {
                EmployerAccountId = employerAccountId,
                ProjectionSource = ProjectionSource.LevyDeclaration
            } );

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
