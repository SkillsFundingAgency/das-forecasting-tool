using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshApprenticeshipsForEmployerHttpFunction 
    {
        [FunctionName("RefreshApprenticeshipsForEmployerHttpFunction")]
        public void Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "RefreshApprenticeshipHttpTrigger")]HttpRequestMessage req,
            [Queue(QueueNames.RefreshEmployersForApprenticeshipUpdate)] ICollector<string> message,
            ILogger logger)

        {
            
                logger.LogDebug("Triggering run of refresh apprenticeships for employers.");
                message.Add("RefreshApprenticeshipsForEmployerHttp");
        

        }
    }
}
