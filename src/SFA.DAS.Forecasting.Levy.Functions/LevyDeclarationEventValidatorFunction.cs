using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Levy.Validation;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventValidatorFunction: IFunction
    {
        [FunctionName("LevyDeclarationEventValidatorFunction")]
        [return:Queue(QueueNames.StoreLevyDeclaration)]
        public static LevySchemeDeclarationUpdatedMessage Run(
            [QueueTrigger(QueueNames.ValidateDeclaration)]LevySchemeDeclarationUpdatedMessage message, 
            TraceWriter writer)
        {
            return FunctionRunner.Run<LevyDeclarationEventValidatorFunction, LevySchemeDeclarationUpdatedMessage>(writer,
                (container, logger) =>
                {
                    var validationResults = container.GetInstance<LevyDeclarationEventValidator>()
                        .Validate(message);
                    if (!validationResults.IsValid)
                    {
                        logger.Warn($"Levy declaration event failed superficial validation. Event: {message.ToJson()}");
                        return null;
                    }

                    logger.Info($"Validated {nameof(LevySchemeDeclarationUpdatedMessage)} for EmployerAccountId: {message.AccountId}");
                    return  message;
                });
        }
    }
}
