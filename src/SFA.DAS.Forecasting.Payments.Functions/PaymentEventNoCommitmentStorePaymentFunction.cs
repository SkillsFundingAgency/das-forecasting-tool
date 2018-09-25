using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventNoCommitmentStorePaymentFunction : IFunction
    {
        [FunctionName("PaymentEventNoCommitmentStorePaymentFunction")]
        [return: Queue(QueueNames.CommitmentProcessor)]
        public static async Task Run(
            [QueueTrigger(QueueNames.PaymentProcessor)]PaymentCreatedMessage paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventNoCommitmentStorePaymentFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Debug($"Storing the payment.  Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.");
                    var handler = container.GetInstance<ProcessEmployerPaymentHandler>();
                    logger.Debug("Resolved handler");
                    await handler.Handle(paymentCreatedMessage, QueueNames.AllowProjection);
                    logger.Info($"Finished storing payment. Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.");
                });
        }
    }
}
