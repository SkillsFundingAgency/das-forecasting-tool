using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Queues;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStoreFunction : IFunction
    {
	    [FunctionName("PaymentEventStoreFunction")]
	    [return: Queue(QueueNames.CommitmentProcessor)]
		public static async Task<PaymentCreatedMessage> Run(
            [QueueTrigger(QueueNames.PaymentProcessor)]PaymentCreatedMessage paymentCreatedMessage, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentEventStoreFunction, PaymentCreatedMessage>(writer,
                async (container, logger) =>
                {
	                var handler = container.GetInstance<ProcessEmployerPaymentHandler>();

					await handler.Handle(paymentCreatedMessage);

	                var queueService = container.GetInstance<QueueService>();
	                var config = container.GetInstance<IApplicationConfiguration>();

					queueService.SendMessageWithVisibilityDelay(paymentCreatedMessage, QueueNames.AllowProjection, TimeSpan.FromSeconds(config.SecondsToWaitToAllowProjections));

                    return await Task.FromResult(paymentCreatedMessage);
                });
        }
    }
}
