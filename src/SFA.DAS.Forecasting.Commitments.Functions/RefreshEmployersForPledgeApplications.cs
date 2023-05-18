using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class RefreshEmployersForPledgeApplications 
    {
        private readonly IPledgesService _pledgesService;

        public RefreshEmployersForPledgeApplications(IPledgesService pledgesService)
        {
            _pledgesService = pledgesService;
        }
        [FunctionName("RefreshEmployersForPledgeApplications")]
        public async Task Run(
            [QueueTrigger(QueueNames.RefreshEmployersForPledgeApplications)] string message,
            [Queue(QueueNames.RefreshPledgeApplicationsForEmployer)] ICollector<RefreshPledgeApplicationsForAccountMessage> updatePledgeApplicationsForAccountOutputMessage,
            ExecutionContext executionContext,
            ILogger logger)
        {
            logger.LogInformation("Getting all pledges account ids...");

            var accountIds = new List<long>();

            try
            {
                accountIds = await _pledgesService.GetAccountIds();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception getting account ids");
                throw;
            }

            var count = 0;
            foreach (var id in accountIds)
            {
                count++;
                updatePledgeApplicationsForAccountOutputMessage.Add(new RefreshPledgeApplicationsForAccountMessage
                {
                    EmployerId = id
                });
            }

            logger.LogInformation($"Added {count} employer id messages to the queue {nameof(QueueNames.RefreshEmployersForPledgeApplications)}.");
        }
    }
}
