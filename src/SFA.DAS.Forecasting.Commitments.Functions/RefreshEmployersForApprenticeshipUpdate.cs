using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class RefreshEmployersForApprenticeshipUpdate 
    {
        private readonly IApprovalsService _approvalsService;

        public RefreshEmployersForApprenticeshipUpdate(IApprovalsService approvalsService)
        {
            _approvalsService = approvalsService;
        }
        
        [FunctionName("RefreshEmployersForApprenticeshipUpdate")]
        public async Task Run(
            [QueueTrigger(QueueNames.RefreshEmployersForApprenticeshipUpdate)] string message,
            [Queue(QueueNames.RefreshApprenticeshipsForEmployer)] ICollector<RefreshApprenticeshipForAccountMessage> updateApprenticeshipForAccountOutputMessage,
            ILogger logger)
        {
            logger.LogInformation("Getting all employer ids...");

            List<long> accountIds;

            try
            {
               accountIds = await _approvalsService.GetEmployerAccountIds();
               logger.LogInformation($"Outer api reports {accountIds.Count} employer accounts");
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
               updateApprenticeshipForAccountOutputMessage.Add(new RefreshApprenticeshipForAccountMessage
               {
                   EmployerId = id
               });
            }

            logger.LogInformation($"Added {count} employer id messages to the queue {nameof(QueueNames.RefreshApprenticeshipsForEmployer)}.");
            
        }
    }
}
