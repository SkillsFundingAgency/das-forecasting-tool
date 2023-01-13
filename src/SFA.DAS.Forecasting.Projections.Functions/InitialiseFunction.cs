using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    public class InitialiseFunction
    {
        [FunctionName("InitialiseFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ExecutionContext executionContext,
            ILogger log)
        {
            
            //TODO: create generic function or use custom binding
            log.LogInformation("Initialising the Projections functions application.");
            
            log.LogInformation("Finished initialising the Projections functions.");
        

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
