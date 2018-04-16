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
        public static void Run([TimerTrigger("0 0 */1 * * *")]TimerInfo myTimer, TraceWriter log,
            [Queue(QueueNames.GetStandards)]ICollector<RefreshCourses> getStandards)
        {
            log.Verbose("Triggering scheduled run of refresh apprenticeship standards.");
            getStandards.Add(new RefreshCourses { RequestTime = DateTime.Now });
            log.Info("Triggered scheduled run of refresh apprenticeship standards.");
        }
    }
}
