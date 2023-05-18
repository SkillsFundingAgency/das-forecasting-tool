using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Commitments.Functions.Application;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class GetPledgeApplicationsForEmployer 
    {
        private readonly IPledgesService _pledgesService;
        private readonly IDeletePledgeApplicationCommitmentsHandler _handler;

        public GetPledgeApplicationsForEmployer(IPledgesService pledgesService, IDeletePledgeApplicationCommitmentsHandler handler)
        {
            _pledgesService = pledgesService;
            _handler = handler;
        }
        [FunctionName("GetPledgeApplicationsForEmployer")]
        public async Task Run(
            [QueueTrigger(QueueNames.RefreshPledgeApplicationsForEmployer)] RefreshPledgeApplicationsForAccountMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ILogger logger)
        {
            
               logger.LogInformation($"Refreshing pledges and applications for employer {message.EmployerId}...");

               try
               {
                   logger.LogInformation($"Deleting pledge application commitments for employer {message.EmployerId}...");
                   
                   await _handler.Handle(message.EmployerId);

                   logger.LogInformation($"Getting pledges and applications for employer {message.EmployerId}...");
                   
                   var pledges = await _pledgesService.GetPledges(message.EmployerId);
                   var applications = new List<Models.Pledges.Application>();

                   foreach (var pledge in pledges)
                   {
                       var pledgeApplications = await _pledgesService.GetApplications(pledge.Id);
                       applications.AddRange(pledgeApplications);
                   }

                   var mapper = new Mapper();

                   var commitments = new List<ApprenticeshipMessage>();

                   foreach (var application in applications)
                   {
                       var numberOfApprenticesUnused = application.NumberOfApprentices - application.NumberOfApprenticesUsed;
                       if (numberOfApprenticesUnused <= 0) continue;
                       logger.LogInformation($"{numberOfApprenticesUnused} of {application.NumberOfApprentices} unused against application {application.Id} pledge {application.PledgeId} employer {message.EmployerId}...");
                       for (var i = 0; i < numberOfApprenticesUnused; i++)
                       {
                           commitments.Add(mapper.Map(application, message.EmployerId));
                       }
                   }

                   foreach (var commitment in commitments)
                   {
                       outputQueueMessage.Add(commitment);
                   }
               }
               catch (Exception ex)
               {
                   logger.LogError(ex, "Exception getting pledges and applications");
                   throw;
               }
        }
    }
}
