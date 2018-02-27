using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
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
	                var handler = container.GetInstance<ProcessEmployerCommitmentHandler>();
					await handler.Handle(paymentCreatedMessage);
                });
        }
    }
}
