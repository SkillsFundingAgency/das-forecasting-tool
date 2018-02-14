using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventAllowProjectionFunction : IFunction
    {
	    [FunctionName("PaymentEventAllowProjectionFunction")]
	    [return: Queue(QueueNames.GeneratePaymentProjection)]
		public static async Task<PaymentCreatedMessage> Run(
            [QueueTrigger(QueueNames.AllowProjection)]PaymentCreatedMessage paymentCreatedMessage, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventStoreFunction, PaymentCreatedMessage>(writer,
                async (container, logger) =>
                {
					logger.Debug("Getting payment declaration handler from container.");
	                var handler = container.GetInstance<AllowAccountProjectionsHandler>();
	                if (handler == null)
		                throw new InvalidOperationException($"Faild to get payment handler from container.");
	                var allowProjections = await handler.Allow(paymentCreatedMessage);
	                if (!allowProjections)
	                {
		                logger.Debug($"Cannot generate the projections, still handling payment events. Employer: {paymentCreatedMessage.EmployerAccountId}");
		                return null;
	                }

	                logger.Info($"Now sending message to trigger the account projections for employer '{paymentCreatedMessage.EmployerAccountId}'");
	                return new PaymentCreatedMessage();
				});
        }
    }
}
