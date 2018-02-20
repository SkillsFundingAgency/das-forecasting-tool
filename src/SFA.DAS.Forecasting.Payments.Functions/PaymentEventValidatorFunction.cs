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
            TraceWriter writer)
        {
            return FunctionRunner.Run<PaymentEventValidatorFunction, PaymentCreatedMessage>(writer, (container, logger) =>
                {
                    var validationResults = container.GetInstance<PaymentEventSuperficialValidator>()
                        .Validate(paymentCreatedMessage);
                    if (validationResults?.Any() ?? false)
                    {
                        logger.Warn($"Payment event failed superficial validation. Event: {paymentCreatedMessage.ToJson()}, Errors:{validationResults.ToJson()}");
                        return null;
                    }

                    logger.Info($"Validated {nameof(PaymentCreatedMessage)} for EmployerAccountId: {paymentCreatedMessage.EmployerAccountId}");
                    return paymentCreatedMessage;
                });
        }
    }
}
