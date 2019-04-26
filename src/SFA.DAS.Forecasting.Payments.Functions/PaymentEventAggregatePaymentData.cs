using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventAggregatePaymentData : IFunction
    {
        [FunctionName("PaymentEventAggregatePaymentDataFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.AggregatePaymentData)]AggregatePaymentDataCommand paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventStorePaymentFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Debug("Getting payment declaration handler from container.");
                    var handler = container.GetInstance<AggregatePaymentDataHandler>();
                    if (handler == null)
                        throw new InvalidOperationException($"Failed to get aggregate payment handler from container.");

                    await handler.Handle(paymentCreatedMessage);
                });
        }
    }
}
