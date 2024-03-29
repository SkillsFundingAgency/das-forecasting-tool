﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class RefreshPledgeApplicationsForEmployer
    {
        [FunctionName("RefreshPledgeApplicationsForEmployer")]
        [return: Queue(QueueNames.RefreshEmployersForPledgeApplications)]
        public static string Run([TimerTrigger("0 40 23 */1 * *")] TimerInfo myTimer, TraceWriter log)
        {
            log.Info("Triggering scheduled run of refresh pledge applications for employers.");
            return "RefreshPledgeApplicationsForEmployer";
        }
    }
}
