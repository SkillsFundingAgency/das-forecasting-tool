using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventCommitmentStoreFunction : IFunction
    {
		[FunctionName("PaymentEventCommitmentStoreFunction")]
		public static async Task Run(
            [QueueTrigger(QueueNames.CommitmentProcessor)]PaymentCreatedMessage paymentCreatedMessage, 
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventStoreFunction, int>(writer,
                async (container, logger) =>
                {
	                var handler = container.GetInstance<ProcessEmployerCommitmentHandler>();
	                
					await handler.Handle(paymentCreatedMessage);

                    return await Task.FromResult(1);
                });
        }
    }
}
