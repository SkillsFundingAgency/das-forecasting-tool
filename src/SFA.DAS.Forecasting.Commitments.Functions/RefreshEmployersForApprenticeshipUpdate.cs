using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class RefreshEmployersForApprenticeshipUpdate : IFunction
    {
        [FunctionName("RefreshEmployersForApprenticeshipUpdate")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshEmployersForApprenticeshipUpdate)] string message,
            [Queue(QueueNames.RefreshApprenticeshipsForEmployer)] ICollector<RefreshApprenticeshipForAccountMessage> updateApprenticeshipForAccountOutputMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {

            await FunctionRunner.Run<RefreshEmployersForApprenticeshipUpdate, string>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Info("Getting all employer ids...");

                   var accountIds = new List<long>();

                   try
                   {
                       var pledgesService = container.GetInstance<IApprovalsService>();
                       accountIds = await pledgesService.GetEmployerAccountIds();
                       logger.Info($"Outer api reports {accountIds.Count} employer accounts");
                   }
                   catch (Exception ex)
                   {
                       logger.Error(ex, "Exception getting pledges");
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
                   var msg = $"Added {count} employer id messages to the queue {nameof(QueueNames.RefreshApprenticeshipsForEmployer)}.";
                   logger.Info(msg);
                   return msg;
               });
        }
    }
}
