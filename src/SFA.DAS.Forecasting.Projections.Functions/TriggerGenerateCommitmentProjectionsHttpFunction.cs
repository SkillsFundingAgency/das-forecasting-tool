using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class TriggerGenerateCommitmentProjectionsHttpFunction
    {
        [FunctionName("TriggerGenerateCommitmentProjectionsHttpFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public static GenerateAccountProjectionCommand Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "TriggerGenerateCommitmentProjections/{employerAccountId}")]CalendarPeriod req,
            long employerAccountId,
            TraceWriter log)
        {
            log.Verbose($"Received http request to generate commitment projections for employer: {employerAccountId}");
            return new GenerateAccountProjectionCommand
            {
                EmployerAccountId = employerAccountId,
                ProjectionSource = ProjectionSource.Commitment,
                StartPeriod = req
            };

        }
    }
}
