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
    public class GetAccountBalanceFunction : IFunction
    {
        [FunctionName("GetAccountBalanceFunction")]
        [return: Queue(QueueNames.BuildProjections)]
        public static async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.GetAccountBalance)]GenerateAccountProjectionCommand message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<GetAccountBalanceFunction, GenerateAccountProjectionCommand>(writer, executionContext, async (container, logger) =>
                {
                    logger.Debug("Resolving GetAccountBalanceHandler from container.");
                    var handler = container.GetInstance<GetAccountBalanceHandler>();
                    if (handler == null)
                        throw new InvalidOperationException("Failed to resolve GetAccountBalanceHandler from container.");
                    await handler.Handle(message);
                    logger.Info($"Finished generating the account projection in response to payment run: {message.EmployerAccountId}");
                    return message;
                });
        }
    }
}