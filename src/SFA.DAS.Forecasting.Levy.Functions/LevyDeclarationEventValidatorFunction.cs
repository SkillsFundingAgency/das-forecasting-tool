using FluentValidation;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventValidatorFunction
    {
        private readonly IValidator<LevySchemeDeclarationUpdatedMessage> _validator;

        public LevyDeclarationEventValidatorFunction(IValidator<LevySchemeDeclarationUpdatedMessage> validator)
        {
            //public class LevyDeclarationEventValidator: AbstractValidator<LevySchemeDeclarationUpdatedMessage>
            _validator = validator;
        }
        
        [FunctionName("LevyDeclarationEventValidatorFunction")]
        [return:Queue(QueueNames.StoreLevyDeclaration)]
        public LevySchemeDeclarationUpdatedMessage Run(
            [QueueTrigger(QueueNames.ValidateDeclaration)]LevySchemeDeclarationUpdatedMessage message, 
            ILogger logger)
        {
            var validationResults = _validator.Validate(message);
            if (!validationResults.IsValid)
            {
                logger.LogWarning($"Levy declaration event failed superficial validation. Employer id: {message.AccountId}, Period: {message.PayrollMonth}, {message.PayrollYear}, Scheme: {message.EmpRef}.");
                return null;
            }

            logger.LogInformation($"Validated {nameof(LevySchemeDeclarationUpdatedMessage)} for EmployerAccountId: {message.AccountId}");
            return  message;
        }
    }
}
