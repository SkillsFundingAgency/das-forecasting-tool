using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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
					logger.Debug("Getting payment declaration handler from container.");
	                var handler = container.GetInstance<AllowAccountProjectionsHandler>();
	                if (handler == null)
		                throw new InvalidOperationException($"Failed to get payment handler from container.");
	                var allowProjections = await handler.Allow(paymentCreatedMessage);
	                if (!allowProjections)
	                {
		                logger.Debug($"Cannot generate the projections, still handling payment events. Employer: {paymentCreatedMessage.EmployerAccountId}");
		                return null;
	                }

	                logger.Info($"Now sending message to trigger the account projections for employer '{paymentCreatedMessage.EmployerAccountId}'");
	                return new GenerateAccountProjectionCommand
                    {
		                EmployerAccountId = paymentCreatedMessage.EmployerAccountId,
                        ProjectionSource = ProjectionSource.PaymentPeriodEnd,
	                };
				});
        }
    }
}
