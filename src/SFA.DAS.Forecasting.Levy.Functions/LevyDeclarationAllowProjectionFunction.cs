using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationAllowProjectionFunction : IFunction
    {
        [FunctionName("LevyDeclarationAllowProjectionFunction")]
        [return:Queue(QueueNames.GenerateProjections)]
        public static async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.AllowProjection)]LevySchemeDeclarationUpdatedMessage message,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationEventStoreFunction, GenerateAccountProjectionCommand>(writer,
                async (container, logger) =>
                {
                    logger.Debug("Getting levy declaration handler from container.");
                    var handler = container.GetInstance<AllowAccountProjectionsHandler>();
                    if (handler == null)
                        throw new InvalidOperationException($"Faild to get levy handler from container.");
                    var allowProjections = await handler.Allow(message);
                    if (!allowProjections)
                    {
                        logger.Debug($"Cannot generate the projections, still handling levy declarations. Employer: {message.AccountId}");
                        return null;
                    }

                    logger.Info($"Now sending message to trigger the account projections for employer '{message.AccountId}'");
                    return new GenerateAccountProjectionCommand
                    {
                        EmployerAccountId = message.AccountId,
                        ProjectionSource = ProjectionSource.LevyDeclaration
                    };
                });
        }
    }
}