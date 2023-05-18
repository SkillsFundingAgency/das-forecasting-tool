using FluentValidation;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Extensions;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class PaymentEventNoCommitmentValidatorFunction
    {
        private readonly IValidator<PaymentCreatedMessage> _validator;

        public PaymentEventNoCommitmentValidatorFunction(IValidator<PaymentCreatedMessage> validator)
        {
            _validator = validator;
            //PastPaymentEventSuperficialValidator : AbstractValidator<PaymentCreatedMessage>
        }
        [FunctionName("PaymentEventNoCommitmentValidatorFunction")]
        [return:Queue(QueueNames.PaymentNoCommitmentProcessor)]
        public PaymentCreatedMessage Run(
            [QueueTrigger(QueueNames.PaymentValidatorNoCommitment)]PaymentCreatedMessage paymentCreatedMessage,
            ILogger logger)
        {
            var validationResults = _validator.Validate(paymentCreatedMessage);

            if (!validationResults.IsValid)
            {
                logger.LogWarning($"Past payment event failed superficial validation. Employer: {paymentCreatedMessage.EmployerAccountId} apprenticeship: {paymentCreatedMessage.ApprenticeshipId}, Errors:{validationResults.ToJson()}");
                return null;
            }

            logger.LogInformation($"Validated past {nameof(PaymentCreatedMessage)} for EmployerAccountId: {paymentCreatedMessage.EmployerAccountId} fundingSource:{paymentCreatedMessage.FundingSource}");
            
            return paymentCreatedMessage;
            
        }
    }
}
