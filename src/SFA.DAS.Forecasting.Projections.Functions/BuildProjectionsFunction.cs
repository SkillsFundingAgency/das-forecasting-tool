using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class BuildProjectionsFunction : IFunction
    {
        [FunctionName("BuildProjectionsFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.BuildProjections)]GenerateAccountProjectionCommand message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GenerateProjectionsFunction>(writer, executionContext, async (container, logger) =>
                {
                    logger.Debug("Resolving BuildAccountProjectionHandler from container.");
                    var handler = container.GetInstance<BuildAccountProjectionHandler>();
                    if (handler == null)
                        throw new InvalidOperationException("Faild to resolve BuildAccountProjectionHandler from container.");
                    await handler.Handle(message);
                    logger.Info($"Finished building the account projection for employer: {message.EmployerAccountId}");
                });
        }
    }
}