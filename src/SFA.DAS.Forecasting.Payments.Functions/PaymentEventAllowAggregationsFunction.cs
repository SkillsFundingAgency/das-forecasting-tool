using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventAllowAggregationsFunction : IFunction
    {
        [FunctionName("PaymentEventAllowAggregationsFunction")]
        [return: Queue(QueueNames.PaymentAggregation)]
		public static async Task<EmployerPeriod> Run(
	        [QueueTrigger(QueueNames.PaymentAggregationAllower)]EmployerPeriod employerPeriod,
			TraceWriter writer)
        {
			return await FunctionRunner.Run<PaymentEventStoreFunction, EmployerPeriod>(writer,
				async (container, logger) =>
				{
					var employerPayment = container.GetInstance<EmployerPayment>();
					var payments = employerPayment.GetPaymentsForEmployerPeriod(employerPeriod.EmployerAccountId, employerPeriod.Month, employerPeriod.Year);

					var trainingCost = container.GetInstance<TrainingCost>();
					var aggregationAllowed = trainingCost.IsAggregationAllowed(payments);

					return aggregationAllowed ? employerPeriod : null;
				});
		}
    }
}
