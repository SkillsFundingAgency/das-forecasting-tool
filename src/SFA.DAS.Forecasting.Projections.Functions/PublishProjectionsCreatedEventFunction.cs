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
    public class PublishProjectionsCreatedEventFunction : IFunction
    {
        [FunctionName("PublishProjectionsCreatedEventFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.PublishAccountProjections)]AccountProjectionCreatedEvent message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<PublishProjectionsCreatedEventFunction>(writer, executionContext, async (container, logger) =>
                {
                    logger.Debug("Resolving PublishAccountProjectionHandler from container.");
                    var handler = container.GetInstance<PublishAccountProjectionHandler>();
                    if (handler == null)
                        throw new InvalidOperationException("Failed to resolve PublishAccountProjectionHandler from container.");
                    await handler.Handle(message);
                    logger.Info($"Finished publishing the account projection event for employer: {message.EmployerAccountId}");
                });
        }
    }
}