using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using SFA.DAS.Forecasting.Levy.Domain.Aggregates;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationEventStoreFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventStoreFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.LevyDeclarationProcessor)]LevyDeclarationEvent levyEvent, 
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationEventStoreFunction, int>(writer,
                async (container, logger) =>
                {
                    var employerLevy = container.GetInstance<EmployerLevy>();
                    await employerLevy.AddDeclaration(levyEvent.EmployerAccountId, levyEvent.PayrollDate, levyEvent.Amount, levyEvent.Scheme, levyEvent.TransactionDate);

                    logger.Info($"Stored {nameof(LevyDeclarationEvent)} for EmployerAccountId: {levyEvent.EmployerAccountId} and PayrollDate: {levyEvent.PayrollDate}");
                    return await Task.FromResult(1);
                });
        }
    }
}
