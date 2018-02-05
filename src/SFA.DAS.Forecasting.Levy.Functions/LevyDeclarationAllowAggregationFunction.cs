using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationAllowAggregationFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventStoreFunction")]
        [return:Queue(QueueNames.AggregateLevyDeclared)]
        public static async Task Run(
            [QueueTrigger(QueueNames.AllowAggregation)]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage,
            
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationEventStoreFunction>(writer,
                async (container, logger) =>
                {
                    logger.Debug("Getting levy declaration handler from container.");
                    var handler = container.GetInstance<AllowLevyDeclarationAggregationHandler>();
                    if (handler == null)
                        throw new InvalidOperationException($"Faild to get levy handler from container.");
                    await handler.Allow(levySchemeUpdatedMessage);
                    logger.Info($"Finished handling levy declaration for EmployerAccountId: {levySchemeUpdatedMessage.AccountId}, PayrollYear: {levySchemeUpdatedMessage.PayrollYear}, month: {levySchemeUpdatedMessage.PayrollMonth}, scheme: {levySchemeUpdatedMessage.EmpRef}");
                });
        }
    }
}