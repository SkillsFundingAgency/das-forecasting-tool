using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventAllowProjectionFunction : IFunction
    {
        [FunctionName("PaymentEventAllowProjectionFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public static async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.AllowProjection)]PaymentCreatedMessage paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventStorePaymentFunction, GenerateAccountProjectionCommand>(writer, executionContext,
                async (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	                telemetry.Debug("PaymentEventAllowProjectionFunction", "Getting payment declaration handler from container.", "FunctionRunner.Run", executionContext.InvocationId);

                    var handler = container.GetInstance<AllowAccountProjectionsHandler>();
	                if (handler == null)
	                {
		                telemetry.Error("PaymentEventAllowProjectionFunction",
			                new InvalidOperationException($"Failed to get payment handler from container."),
			                "Failed to get payment handler from container.", 
							"FunctionRunner.Run");
		                throw new InvalidOperationException($"Failed to get payment handler from container.");
	                }
	                var allowProjections = await handler.Allow(paymentCreatedMessage);
                    if (!allowProjections)
                    {
	                    telemetry.Debug("PaymentEventAllowProjectionFunction", $"Cannot generate the projections, still handling payment events. Employer: {paymentCreatedMessage.EmployerAccountId}", "FunctionRunner.Run", executionContext.InvocationId);

                        return null;
                    }

	                telemetry.Info("PaymentEventAllowProjectionFunction", $"Now sending message to trigger the account projections for employer '{paymentCreatedMessage.EmployerAccountId}', period: {paymentCreatedMessage.CollectionPeriod?.Id}, {paymentCreatedMessage.CollectionPeriod?.Month}", "FunctionRunner.Run", executionContext.InvocationId);

                    return new GenerateAccountProjectionCommand
                    {
                        EmployerAccountId = paymentCreatedMessage.EmployerAccountId,
                        ProjectionSource = ProjectionSource.PaymentPeriodEnd,
                    };
                });
        }
    }
}
