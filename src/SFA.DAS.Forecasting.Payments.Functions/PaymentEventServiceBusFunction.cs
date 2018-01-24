using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Payments.Application.Messages;
using SFA.DAS.Forecasting.Payments.Application.Validation;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventServiceBusFunction : IFunction
	{
	    [FunctionName("PaymentEventServiceBusFunction")]
	    [return: Queue("myQueueName")]
	    public static async Task<PaymentEvent> Run(
		    [ServiceBusTrigger("LevyDeclaration", "mysubscription", AccessRights.Manage)]PaymentEvent paymentEvent,
		    TraceWriter writer)
	    {
		    return await FunctionRunner.Run<PaymentEventServiceBusFunction, PaymentEvent>(writer,
			    async (container, logger) =>
			    {
				    logger.Info($"Added {nameof(PaymentEvent)} to queue: myQueueName,  for EmployerAccountId: {paymentEvent?.EmployerAccountId}");
				    return await Task.FromResult(paymentEvent);
			    });
	    }
	}
}
