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
    public class GeneratePaymentProjectionsFunction : IFunction
    {
        [FunctionName("GeneratePaymentProjectionsFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.GeneratePaymentProjections)]GeneratePaymentAccountProjection generateProjection,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GeneratePaymentProjectionsFunction>(writer,
                async (container, logger) =>
                {
                    logger.Debug("Getting account projections handler from container.");
                    var handler = container.GetInstance<GeneratePaymentAccountProjectionHandler>();
                    if (handler == null)
                        throw new InvalidOperationException($"Faild to get GeneratePaymentAccountProjectionHandler from container.");
                    await handler.Handle(generateProjection);
                    logger.Info($"Finished generating the account projection in response to payment run: {generateProjection.EmployerAccountId}");
                });
        }
    }
}