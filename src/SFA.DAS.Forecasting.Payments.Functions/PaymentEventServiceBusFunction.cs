using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventServiceBusFunction : IFunction
	{
	    [FunctionName("PaymentEventServiceBusFunction")]
	    [return: Queue(QueueNames.PaymentValidator)]
	    public static async Task<PaymentCreatedMessage> Run(
		    [ServiceBusTrigger("LevyPeriod", "mysubscription", AccessRights.Manage)]PaymentCreatedMessage paymentCreatedMessage,
		    TraceWriter writer)
	    {
		    return await FunctionRunner.Run<PaymentEventServiceBusFunction, PaymentCreatedMessage>(writer,
			    async (container, logger) =>
			    {
				    logger.Info($"Added {nameof(PaymentCreatedMessage)} to queue: {QueueNames.PaymentValidator},  for EmployerAccountId: {paymentCreatedMessage?.EmployerAccountId}");
				    return await Task.FromResult(paymentCreatedMessage);
			    });
	    }
	}
}
