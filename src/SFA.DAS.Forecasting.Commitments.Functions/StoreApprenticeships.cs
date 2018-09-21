using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class StoreApprenticeships : IFunction
    {
        [FunctionName("StoreApprenticeships")]
        public static async Task Run(
            [QueueTrigger(QueueNames.StoreApprenticeships)]ApprenticeshipMessage message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<StoreApprenticeships>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Debug($"Storing apprenticeship. Account: {message.EmployerAccountId}, apprenticeship id: {message.ApprenticeshipId}");
                    var handler = container.GetInstance<StoreCommitmentHandler>();
                    await handler.Handle(message,QueueNames.AllowProjection);
                    logger.Info($"Stored commitment. Apprenticeship id: {message.ApprenticeshipId}");   
                });
        }
    }
}
