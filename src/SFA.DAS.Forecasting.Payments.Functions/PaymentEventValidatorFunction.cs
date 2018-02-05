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
        public static async Task<PaymentEvent> Run(
            [QueueTrigger(QueueNames.PaymentValidator)]PaymentEvent paymentEvent, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventValidatorFunction, PaymentEvent>(writer,
                (container, logger) =>
                {
                    var validationResults = container.GetInstance<PaymentEventSuperficialValidator>()
                        .Validate(paymentEvent);
                    if (validationResults?.Any() ?? false)
                    {
                        logger.Warn($"Payment event failed superficial validation. Event: {paymentEvent.ToJson()}");
                        return null;
                    }

                    logger.Info($"Validated {nameof(PaymentEvent)} for EmployerAccountId: {paymentEvent.EmployerAccountId}");
                    return Task.FromResult(paymentEvent);
                });
        }
    }
}
