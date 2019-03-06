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
    public class LevyDeclarationEventNoProjectionValidatorFunction: IFunction
    {
        [FunctionName("LevyDeclarationEventNoProjectionValidatorFunction")]
        [return:Queue(QueueNames.StoreLevyDeclarationNoProjection)]
        public static LevySchemeDeclarationUpdatedMessage Run(
            [QueueTrigger(QueueNames.ValidateLevyDeclarationNoProjection)]LevySchemeDeclarationUpdatedMessage message, 
            TraceWriter writer, ExecutionContext executionContext)
        {
            return FunctionRunner.Run<LevyDeclarationEventNoProjectionValidatorFunction, LevySchemeDeclarationUpdatedMessage>(writer, executionContext,
                (container, logger) =>
                {
                    var validationResults = container.GetInstance<LevyDeclarationEventValidator>()
                        .Validate(message);
                    if (!validationResults.IsValid)
                    {
                        logger.Warn($"Past levy declaration event failed superficial validation. Employer id: {message.AccountId}, Period: {message.PayrollMonth}, {message.PayrollYear}, Scheme: {message.EmpRef}.");
                        return null;
                    }

                    logger.Info($"Validated {nameof(LevySchemeDeclarationUpdatedMessage)} for EmployerAccountId: {message.AccountId}");
                    return  message;
                });
        }
    }
}
