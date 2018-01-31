using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Functions.Framework;

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
                    var employerLevy = container.GetInstance<EmployerPayment>();
                    await employerLevy.AddPayment(paymentEvent.Id, 
						int.Parse(paymentEvent.EmployerAccountId), 
						paymentEvent.Ukprn, 
						paymentEvent.ApprenticeshipId, 
						paymentEvent.Amount);

                    logger.Info($"Stored {nameof(PaymentEvent)} for EmployerAccountId: {paymentEvent.EmployerAccountId}");
                    return await Task.FromResult(1);
                });
        }
    }
}
