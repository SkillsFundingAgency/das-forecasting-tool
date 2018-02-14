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
    public class GenerateLevyProjectionsFunction : IFunction
    {
        [FunctionName("GenerateLevyProjectionsFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.GenerateLevyProjections)]GenerateLevyAccountProjection generateProjection,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GenerateLevyProjectionsFunction>(writer,
                async (container, logger) =>
                {
                    logger.Debug("Getting account projections handler from container.");
                    var handler = container.GetInstance<GenerateLevyAccountProjectionHandler>();
                    if (handler == null)
                        throw new InvalidOperationException($"Faild to get levy handler from container.");
                    await handler.Handle(generateProjection);
                    logger.Info($"Finished generating the account projection in response to Levy declaration: {generateProjection.EmployerAccountId}");
                });
        }
    }
}