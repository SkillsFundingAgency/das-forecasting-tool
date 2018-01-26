using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using SFA.DAS.Forecasting.Levy.Application.Validation;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventValidatorFunction: IFunction
    {
        [FunctionName("LevyDeclarationEventValidatorFunction")]
        [return:Queue(QueueNames.LevyDeclarationProcessor)]
        public static async Task<LevyDeclarationEvent> Run(
            [QueueTrigger(QueueNames.LevyDeclarationValidator)]LevyDeclarationEvent levyDeclarationEvent, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationEventValidatorFunction, LevyDeclarationEvent>(writer,
                (container, logger) =>
                {
                    var validationResults = container.GetInstance<LevyDeclarationEventValidator>()
                        .Validate(levyDeclarationEvent);
                    if (!validationResults.IsValid)
                    {
                        logger.Warn($"Levy declaration event failed superficial validation. Event: {levyDeclarationEvent.ToJson()}");
                        return null;
                    }

                    logger.Info($"Validated {nameof(LevyDeclarationEvent)} for EmployerAccountId: {levyDeclarationEvent.EmployerAccountId}");
                    return  Task.FromResult(levyDeclarationEvent);
                });
        }
    }
}
