using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventServiceBusFunction 
	{
	    [FunctionName("PaymentEventServiceBusFunction")]
	    [return: Queue(QueueNames.PaymentValidator)]
	    public static async Task<PaymentCreatedMessage> Run(
		    [ServiceBusTrigger("LevyPeriod", "mysubscription")]PaymentCreatedMessage paymentCreatedMessage,
		    ILogger logger)
	    {
		    
		    logger.LogInformation($"Added {nameof(PaymentCreatedMessage)} to queue: {QueueNames.PaymentValidator},  for EmployerAccountId: {paymentCreatedMessage?.EmployerAccountId}");
		    return await Task.FromResult(paymentCreatedMessage);
			
	    }
	}
}
