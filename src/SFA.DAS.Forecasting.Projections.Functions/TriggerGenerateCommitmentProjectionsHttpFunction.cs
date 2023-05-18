using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class TriggerGenerateCommitmentProjectionsHttpFunction
    {
        [FunctionName("TriggerGenerateCommitmentProjectionsHttpFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public GenerateAccountProjectionCommand Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "TriggerGenerateCommitmentProjections/{employerAccountId}")]CalendarPeriod req,
            long employerAccountId,
            ILogger log)
        {
            log.LogDebug($"Received http request to generate commitment projections for employer: {employerAccountId}");
            return new GenerateAccountProjectionCommand
            {
                EmployerAccountId = employerAccountId,
                ProjectionSource = ProjectionSource.Commitment,
                StartPeriod = req
            };

        }
    }
}
