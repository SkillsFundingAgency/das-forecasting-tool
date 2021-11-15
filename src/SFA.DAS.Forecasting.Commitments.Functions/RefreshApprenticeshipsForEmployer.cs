using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshApprenticeshipsForEmployer
    {
        [FunctionName("RefreshApprenticeshipCoursesFunction")]
        [return: Queue(QueueNames.RefreshEmployersForApprenticeshipUpdate)]
        public static string Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Verbose("Triggering scheduled run of refresh apprenticeships for employers.");
            return "RefreshApprenticeshipsForEmployer";
        }
    }
}
