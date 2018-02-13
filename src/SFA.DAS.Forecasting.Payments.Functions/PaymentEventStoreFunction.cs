using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
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
	                var mapper = container.GetInstance<PaymentMapper>();

					await handler.Handle(mapper.MapToPayment(paymentCreatedMessage));

                    return await Task.FromResult(paymentCreatedMessage);
                });
        }
    }
}
