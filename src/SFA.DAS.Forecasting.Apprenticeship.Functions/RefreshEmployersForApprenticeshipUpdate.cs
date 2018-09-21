using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class RefreshEmployersForApprenticeshipUpdate : IFunction
    {
        [FunctionName("RefreshEmployersForApprenticeshipUpdate")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshEmployersForApprenticeshipUpdate)]string message,
            [Queue(QueueNames.RefreshApprenticeshipsForEmployer)] ICollector<RefreshApprenticeshipForAccountMessage> updateApprenticeshipForAccountOutputMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<RefreshEmployersForApprenticeshipUpdate, string>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Debug("Getting all employer ids...");
                   var employerCommitmentsApi = container.GetInstance<IEmployerCommitmentApi>();
                   var allEmployerIds = await employerCommitmentsApi.GetAllEmployerAccountIds();

                   var count = 0;
                   foreach (var id in allEmployerIds)
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
