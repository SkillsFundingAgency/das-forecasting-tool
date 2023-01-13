using FluentValidation;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Extensions;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class PaymentEventValidatorFunction
    {
        private readonly IValidator<PaymentCreatedMessage> _validator;

        public PaymentEventValidatorFunction(IValidator<PaymentCreatedMessage> validator)
        {
            _validator = validator;
            //PaymentEventSuperficialValidator : AbstractValidator<PaymentCreatedMessage>
        }
        [FunctionName("PaymentEventValidatorFunction")]
        [return:Queue(QueueNames.PaymentProcessor)]
        public PaymentCreatedMessage Run(
            [QueueTrigger(QueueNames.PaymentValidator)]PaymentCreatedMessage paymentCreatedMessage,
            ILogger logger)
        {
            var validationResults = _validator
                .Validate(paymentCreatedMessage);

            if (!validationResults.IsValid)
            {
                logger.LogWarning($"Payment event failed superficial validation. Employer: {paymentCreatedMessage.EmployerAccountId} apprenticeship: {paymentCreatedMessage.ApprenticeshipId}, Errors:{validationResults.ToJson()}");
                return null;
            }

            logger.LogInformation($"Validated {nameof(PaymentCreatedMessage)} for EmployerAccountId: {paymentCreatedMessage.EmployerAccountId} fundingSource:{paymentCreatedMessage.FundingSource}");
            
            return paymentCreatedMessage;
            
        }
    }
}
