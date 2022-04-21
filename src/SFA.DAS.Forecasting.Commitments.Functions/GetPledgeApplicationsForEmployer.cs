using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class GetPledgeApplicationsForEmployer : IFunction
    {
        [FunctionName("GetPledgeApplicationsForEmployer")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshPledgeApplicationsForEmployer)] RefreshPledgeApplicationsForAccountMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GetPledgeApplicationsForEmployer>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Info($"Refreshing pledges and applications for employer {message.EmployerId}...");

                   try
                   {
                       logger.Info($"Deleting pledge application commitments for employer {message.EmployerId}...");
                       var handler = container.GetInstance<DeletePledgeApplicationCommitmentsHandler>();
                       await handler.Handle(message.EmployerId);

                       logger.Info($"Getting pledges and applications for employer {message.EmployerId}...");
                       var pledgesService = container.GetInstance<IPledgesService>();
                       var pledges = await pledgesService.GetPledges(message.EmployerId);
                       var applications = new List<Models.Pledges.Application>();

                       foreach (var pledge in pledges)
                       {
                           var pledgeApplications = await pledgesService.GetApplications(pledge.Id);
                           applications.AddRange(pledgeApplications);
                       }

                       var mapper = new Mapper();

                       var commitments = new List<ApprenticeshipMessage>();

                       foreach (var application in applications)
                       {
                           var numberOfApprenticesUnused = application.NumberOfApprentices - application.NumberOfApprenticesUsed;
                           if (numberOfApprenticesUnused <= 0) continue;
                           logger.Info($"{numberOfApprenticesUnused} of {application.NumberOfApprentices} unused against application {application.Id} pledge {application.PledgeId} employer {message.EmployerId}...");
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
                       logger.Error(ex, "Exception getting pledges and applications");
                       throw;
                   }
               });
        }
    }
}
