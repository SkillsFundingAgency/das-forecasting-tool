using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Messages.ApprenticeshipCourses;

namespace SFA.DAS.Forecasting.ApprenticeshipCourses.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class GetStandardsFunction
    {
        [FunctionName("GetStandardsFunction")]
        public static void Run([QueueTrigger(QueueNames.GetStandards)] RefreshCourses message,
            TraceWriter log,
            [Queue(QueueNames.StoreStandard)]ICollector<RefreshCourses> storeStandards)
        {
            log.Verbose("Triggering scheduled run of refresh apprenticeship standards.");
            //getStandards.Add(new RefreshCourses { RequestTime = DateTime.Now });
            log.Info("Triggered scheduled run of refresh apprenticeship standards.");
        }
    }
}