using FluentValidation;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventNoProjectionValidatorFunction
    {
        private readonly IValidator<LevySchemeDeclarationUpdatedMessage> _validator;

        public LevyDeclarationEventNoProjectionValidatorFunction(IValidator<LevySchemeDeclarationUpdatedMessage> validator)
        {
            _validator = validator;
            //LevyDeclarationEventValidator: AbstractValidator<LevySchemeDeclarationUpdatedMessage>
        }
        [FunctionName("LevyDeclarationEventNoProjectionValidatorFunction")]
        [return:Queue(QueueNames.StoreLevyDeclarationNoProjection)]
        public LevySchemeDeclarationUpdatedMessage Run(
            [QueueTrigger(QueueNames.ValidateLevyDeclarationNoProjection)]LevySchemeDeclarationUpdatedMessage message, 
            ILogger logger, ExecutionContext executionContext)
        {
            var validationResults = _validator
                .Validate(message);
            if (!validationResults.IsValid)
            {
                logger.LogWarning($"Past levy declaration event failed superficial validation. Employer id: {message.AccountId}, Period: {message.PayrollMonth}, {message.PayrollYear}, Scheme: {message.EmpRef}.");
                return null;
            }

            logger.LogInformation($"Validated {nameof(LevySchemeDeclarationUpdatedMessage)} for EmployerAccountId: {message.AccountId}");
            return  message;
            
        }
    }
}
