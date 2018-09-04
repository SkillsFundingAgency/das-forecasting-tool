using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshApprenticeshipsForEmployer
    {
        [FunctionName("RefreshApprenticeshipCoursesFunction")]
        [return: Queue(QueueNames.GetApprenticeshipsForEmployer)]
        public static string Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Verbose("Triggering scheduled run of refresh apprenticeships for employers.");
            return "";
        }
    }
}
