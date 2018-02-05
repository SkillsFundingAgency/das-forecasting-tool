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
        public static async Task Run(
	        [QueueTrigger(QueueNames.PaymentAggregationAllower)]EmployerPeriod employerPeriod,
	        [Queue(QueueNames.PaymentAggregation)] ICollector<EmployerPeriod> collector, 
			TraceWriter writer)
        {
			await FunctionRunner.Run<PaymentEventStoreFunction, int>(writer,
				async (container, logger) =>
				{
					var employerPayment = container.GetInstance<EmployerPayment>();
					var payments = employerPayment.GetPaymentsForEmployerPeriod(employerPeriod.EmployerAccountId, employerPeriod.Month, employerPeriod.Year);

					var trainingCost = container.GetInstance<TrainingCost>();
					var aggregationAllowed = trainingCost.IsAggregationAllowed(payments);

					if (aggregationAllowed)
					{
						collector.Add(employerPeriod);
					}

					return await Task.FromResult(1);
				});
		}
    }
}
