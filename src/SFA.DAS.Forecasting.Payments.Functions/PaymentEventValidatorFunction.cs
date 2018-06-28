using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class PaymentEventValidatorFunction: IFunction
    {
        [FunctionName("PaymentEventValidatorFunction")]
        [return:Queue(QueueNames.PaymentProcessor)]
        public static PaymentCreatedMessage Run(
            [QueueTrigger(QueueNames.PaymentValidator)]PaymentCreatedMessage paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return FunctionRunner.Run<PaymentEventValidatorFunction, PaymentCreatedMessage>(writer, executionContext, (container, logger) =>
                {
                    var validationResults = container.GetInstance<PaymentEventSuperficialValidator>()
                        .Validate(paymentCreatedMessage);

                    if (!validationResults.IsValid)
                    {
                        writer.Warning($"Payment event failed superficial validation. Employer: {paymentCreatedMessage.EmployerAccountId} apprenticeship: {paymentCreatedMessage.ApprenticeshipId}, Errors:{validationResults.ToJson()}");
                        logger.Warn($"Payment event failed superficial validation. Employer: {paymentCreatedMessage.EmployerAccountId} apprenticeship: {paymentCreatedMessage.ApprenticeshipId}, Errors:{validationResults.ToJson()}");
                        return null;
                    }

                    writer.Info($"Validated {nameof(PaymentCreatedMessage)} for EmployerAccountId: {paymentCreatedMessage.EmployerAccountId} fundingSource:{paymentCreatedMessage.FundingSource}");
                    return paymentCreatedMessage;
                });
        }
    }
}
