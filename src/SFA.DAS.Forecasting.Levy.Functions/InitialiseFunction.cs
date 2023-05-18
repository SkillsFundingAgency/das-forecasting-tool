using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class InitialiseFunction
    {
        [FunctionName("InitialiseFunction")]
        public async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger logger)
        {
            
            logger.LogInformation("Initialising the Levy functions.");
            
            logger.LogInformation("Finished initialising the Levy functions.");
        

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
