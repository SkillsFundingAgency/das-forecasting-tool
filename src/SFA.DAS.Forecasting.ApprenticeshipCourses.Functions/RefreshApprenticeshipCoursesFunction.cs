using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class RefreshApprenticeshipCoursesFunction
    {
        [FunctionName("RefreshApprenticeshipCoursesFunction")]
        [return:Queue(QueueNames.GetCourses)]
        public static RefreshCourses Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Verbose("Triggering scheduled run of refresh apprenticeship courses.");
            return new RefreshCourses {
                RequestTime = DateTime.Now,
                CourseType = CourseType.Standards | CourseType.Frameworks
            };
        }
    }
}
