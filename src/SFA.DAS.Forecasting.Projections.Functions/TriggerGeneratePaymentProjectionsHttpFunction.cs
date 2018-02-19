using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class TriggerGeneratePaymentProjectionsHttpFunction
    {
        [FunctionName("TriggerGeneratePaymentProjectionsHttpFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "TriggerGeneratePaymentProjections/{employerAccountId}")]HttpRequestMessage req, long employerAccountId,
            [Queue(QueueNames.GenerateProjections)]ICollector<GenerateAccountProjectionCommand> messages,TraceWriter log)
        {
            messages.Add(new GenerateAccountProjectionCommand
            {
                EmployerAccountId = employerAccountId,
                ProjectionSource = ProjectionSource.PaymentPeriodEnd
            });

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}