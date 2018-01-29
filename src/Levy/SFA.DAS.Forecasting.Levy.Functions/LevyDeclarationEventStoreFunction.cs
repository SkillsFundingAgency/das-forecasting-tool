using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using SFA.DAS.Forecasting.Levy.Domain.Aggregates;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventStoreFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventStoreFunction")]
        [return:Queue()]
        public static async Task Run(
            [QueueTrigger(QueueNames.StoreLevyDeclaration)]LevyDeclarationEvent levyEvent,
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationEventStoreFunction>(writer,
                async (container, logger) =>
                {
                    var repository = container.GetInstance<ILevyPeriodRepository>();
                    var levyPeriod = await repository.Get(levyEvent.EmployerAccountId, levyEvent.PayrollYear, levyEvent.PayrollMonth ?? 0);
                    levyPeriod.AddDeclaration(levyEvent.EmployerAccountId, levyEvent.PayrollYear, levyEvent.PayrollMonth ?? 0, levyEvent.Amount, levyEvent.Scheme, levyEvent.TransactionDate);
                    await repository.StoreLevyPeriod(levyPeriod);
                    logger.Info($"Stored {nameof(LevyDeclarationEvent)} for EmployerAccountId: {levyEvent.EmployerAccountId} and PayrollYear: {levyEvent.PayrollYear}");
                });
        }
    }
}
