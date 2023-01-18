using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshApprenticeshipsForEmployer
    {
        [FunctionName("RefreshEmployersForApprenticeshipTimerUpdate")]
        [return: Queue(QueueNames.RefreshEmployersForApprenticeshipUpdate)]
        public string Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Triggering scheduled run of refresh apprenticeships for employers.");
            return "RefreshApprenticeshipsForEmployer";
        }
    }
}
