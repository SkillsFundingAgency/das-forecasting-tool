using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

//TODO FAI-625
namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshApprenticeshipCoursesFunction
    {
        [FunctionName("RefreshApprenticeshipCoursesFunction")]
        [return:Queue(QueueNames.GetCourses)]
        public static async Task<RefreshCourses> RunAsync([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogDebug("Triggering scheduled run of refresh apprenticeship courses.");
            return new RefreshCourses {
                RequestTime = DateTime.Now,
                CourseType = CourseType.Standards | CourseType.Frameworks
            };
        }
    }
}
