using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStorePaymentFunction : IFunction
    {
        [FunctionName("PaymentEventStorePaymentFunction")]
        [return: Queue(QueueNames.CommitmentProcessor)]
        public static async Task<PaymentCreatedMessage> Run(
            [QueueTrigger(QueueNames.PaymentProcessor)]PaymentCreatedMessage paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventStorePaymentFunction, PaymentCreatedMessage>(writer, executionContext,
                async (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	                telemetry.Debug("PaymentEventStorePaymentFunction", $"Storing the payment.  Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.", "FunctionRunner.Run", executionContext.InvocationId);

                    var handler = container.GetInstance<ProcessEmployerPaymentHandler>();
	                telemetry.Debug("PaymentEventStorePaymentFunction", "Resolved handler", "FunctionRunner.Run", executionContext.InvocationId);

                    await handler.Handle(paymentCreatedMessage, QueueNames.AllowProjection);
	                telemetry.Info("PaymentEventStorePaymentFunction", $"Finished storing payment. Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.", "FunctionRunner.Run", executionContext.InvocationId);

                    return paymentCreatedMessage;
                });
        }
    }
}
