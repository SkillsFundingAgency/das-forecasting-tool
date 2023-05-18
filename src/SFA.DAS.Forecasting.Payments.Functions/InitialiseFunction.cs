using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class InitialiseFunction
    {
        [FunctionName("InitialiseFunction")]
        public HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Initialising the Payments functions.");
            log.LogInformation("Finished initialising the Payments functions.");
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
