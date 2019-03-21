using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventAllowAggregatePaymentData : IFunction
    {
        [FunctionName("PaymentEventAllowAggregatePaymentDataFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.AllowPaymentAggregation)]PaymentCreatedMessage paymentCreatedMessage,
            [Queue(QueueNames.AggregatePaymentData)] ICollector<AggregatePaymentDataCommand> collector,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventStorePaymentFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Debug("Getting payment declaration handler from container.");
                    var handler = container.GetInstance<AllowAccountProjectionsHandler>();
                    if (handler == null)
                        throw new InvalidOperationException($"Failed to get payment handler from container.");

                    var allowedEmployerAccounts = (await handler.AllowedEmployerAccountIds(paymentCreatedMessage)).ToList();

                    if (allowedEmployerAccounts.Any())
                    {
                        foreach (var allowedEmployerAccount in allowedEmployerAccounts)
                        {
                            logger.Info($"Now sending message to trigger the payment aggregation for '{allowedEmployerAccount}', period: {paymentCreatedMessage.CollectionPeriod?.Id}, {paymentCreatedMessage.CollectionPeriod?.Month}");
                            
                            collector.Add(new AggregatePaymentDataCommand
                            {
                                EmployerAccountId = allowedEmployerAccount,
                                CollectionPeriodYear = paymentCreatedMessage.CollectionPeriod.Year,
                                CollectionPeriodMonth = paymentCreatedMessage.CollectionPeriod.Month
                            });
                        }
                    }
                    else
                    {
                        logger.Debug($"Cannot generate the payment aggregation, still handling payment events. Employer: {paymentCreatedMessage.EmployerAccountId}");
                    }
                });
        }
    }
}