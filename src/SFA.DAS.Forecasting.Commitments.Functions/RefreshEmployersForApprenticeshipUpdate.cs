using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class RefreshEmployersForApprenticeshipUpdate : IFunction
    {
        static RefreshEmployersForApprenticeshipUpdate()
        {
            ApplicationHelper.AssemblyBindingRedirect();
        }

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
                   logger.Debug("Getting all employer ids...");
                   var employerCommitmentsApi = container.GetInstance<ICommitmentsApiClient>();
                   var allEmployerIds = await employerCommitmentsApi.GetAllCohortAccountIds();

                   var count = 0;
                   foreach (var id in allEmployerIds.AccountIds)
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
