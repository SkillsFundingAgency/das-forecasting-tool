using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStoreCommitmentFunction : IFunction
    {
		[FunctionName("PaymentEventStoreCommitmentFunction")]
		public static async Task Run(
            [QueueTrigger(QueueNames.CommitmentProcessor)]PaymentCreatedMessage paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventStoreCommitmentFunction>(writer, executionContext,
                async (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	                telemetry.Debug("PaymentEventStoreCommitmentFunction", $"Storing commitment. Account: {paymentCreatedMessage.EmployerAccountId}, apprenticeship id: {paymentCreatedMessage.ApprenticeshipId}", "FunctionRunner.Run", executionContext.InvocationId);

	                var handler = container.GetInstance<StoreCommitmentHandler>();
					await handler.Handle(paymentCreatedMessage);

	                telemetry.Info("PaymentEventStoreCommitmentFunction", $"Stored commitment. Apprenticeship id: {paymentCreatedMessage.ApprenticeshipId}", "FunctionRunner.Run", executionContext.InvocationId);
                });
        }
    }
}
