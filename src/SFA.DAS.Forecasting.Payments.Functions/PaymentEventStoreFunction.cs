using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Queues;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Functions.Framework;
using CollectionPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.CollectionPeriod;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStoreFunction : IFunction
    {
	    public EarningDetails EarningDetails { get; set; }
	    public CollectionPeriod CollectionPeriod { get; set; }
		[FunctionName("PaymentEventStoreFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.PaymentProcessor)]PaymentEvent paymentEvent, 
            TraceWriter writer)
        {
            await FunctionRunner.Run<PaymentEventStoreFunction, int>(writer,
                async (container, logger) =>
                {
                    var employerPayment = container.GetInstance<EmployerPayment>();
	                var paymentMapper = container.GetInstance<PaymentMapper>();

					await employerPayment.AddPayment(paymentMapper.MapToPayment(paymentEvent));

	                logger.Info($"Stored {nameof(PaymentEvent)} for EmployerAccountId: {paymentEvent.EmployerAccountId}");

					var employerPeriod = new EmployerPeriod
					{
						EmployerAccountId = paymentEvent.EmployerAccountId.ToString(),
						Month = paymentEvent.CollectionPeriod.Month,
						Year = paymentEvent.CollectionPeriod.Year
					};

					container.GetInstance<QueueService>().SendMessageWithVisibilityDelay(employerPeriod, QueueNames.PaymentAggregationAllower, 10);

                    return await Task.FromResult(1);
                });
        }
    }
}
