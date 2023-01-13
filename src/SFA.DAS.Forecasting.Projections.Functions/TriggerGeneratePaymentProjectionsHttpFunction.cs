using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class TriggerGeneratePaymentProjectionsHttpFunction
    {
        [FunctionName("TriggerGeneratePaymentProjectionsHttpFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public static GenerateAccountProjectionCommand Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "TriggerGeneratePaymentProjections/{employerAccountId}")]CalendarPeriod req, 
            long employerAccountId,
            ILogger log)
        {
            log.LogDebug($"Received http request to generate payments projections for employer: {employerAccountId}");
            return new GenerateAccountProjectionCommand
            {
                EmployerAccountId = employerAccountId,
                ProjectionSource = ProjectionSource.PaymentPeriodEnd,
                StartPeriod = req
            };

        }
    }
}