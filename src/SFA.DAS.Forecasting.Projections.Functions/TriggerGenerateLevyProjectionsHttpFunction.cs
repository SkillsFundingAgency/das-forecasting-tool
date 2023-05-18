using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class TriggerGenerateLevyProjectionsHttpFunction
    {
        [FunctionName("TriggerGenerateLevyProjectionsHttpFunction")]
        [return:Queue(QueueNames.GenerateProjections)]
        public static GenerateAccountProjectionCommand Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "TriggerGenerateLevyProjections/{employerAccountId}")] CalendarPeriod req, 
            long employerAccountId,
            ILogger log)
        {
            log.LogDebug($"Received http request to generate projections for employer: {employerAccountId}");
            return new GenerateAccountProjectionCommand
            {
                EmployerAccountId = employerAccountId,
                ProjectionSource = ProjectionSource.LevyDeclaration,
                StartPeriod = req
            };
        }
    }
}
