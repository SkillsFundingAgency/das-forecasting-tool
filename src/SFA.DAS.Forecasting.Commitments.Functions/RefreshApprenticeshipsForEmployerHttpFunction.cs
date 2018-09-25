using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshApprenticeshipsForEmployerHttpFunction : IFunction
    {
        [FunctionName("RefreshApprenticeshipsForEmployerHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "RefreshApprenticeshipHttpTrigger")]HttpRequestMessage req,
            [Queue(QueueNames.RefreshEmployersForApprenticeshipUpdate)] ICollector<string> message,
            ExecutionContext executionContext,
            TraceWriter writer)

        {
            return FunctionRunner.Run<RefreshApprenticeshipsForEmployerHttpFunction, string>(writer, executionContext,
                (container, logger) =>
                {
                    logger.Debug("Triggering run of refresh apprenticeships for employers.");
                    message.Add("RefreshApprenticeshipsForEmployerHttp");
                    return "Triggering run of refresh apprenticeships for employers.";
                });

        }
    }
}
