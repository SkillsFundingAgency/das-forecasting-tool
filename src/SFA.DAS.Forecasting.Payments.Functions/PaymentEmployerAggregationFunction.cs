using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Domain.Payments.Entities;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class PaymentEmployerAggregationFunction : IFunction
    {
        [FunctionName("PaymentEmployerAggregationFunction")]
        [return: Queue(QueueNames.PaymentAggregationProcessor)]
		public static async Task<EmployerTotalCostOfTraining> Run(
	        [QueueTrigger(QueueNames.PaymentAggregation)]EmployerPeriod employerPeriod,
	        TraceWriter writer)
		{
            return await FunctionRunner.Run<PaymentEmployerAggregationFunction, EmployerTotalCostOfTraining>(writer,
                async (container, logger) =>
                {
	                var employerPayment = container.GetInstance<EmployerPayment>();
					var payments = employerPayment.GetPaymentsForEmployerPeriod(employerPeriod.EmployerAccountId, employerPeriod.Month, employerPeriod.Year);

	                var trainingCost = container.GetInstance<TrainingCost>();
					var employerTotalCostOfTraining = trainingCost.AggregateEmployerPayments(payments);

                    return employerTotalCostOfTraining;
                });
        }
    }
}
