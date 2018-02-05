using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Domain.Payments.Entities;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class PaymentEmployerAggregationStoreFunction : IFunction
    {
        [FunctionName("PaymentEmployerAggregationStoreFunction")]
		public static async Task Run(
	        [QueueTrigger(QueueNames.PaymentAggregationProcessor)]EmployerTotalCostOfTraining employerTotalCostOfTraining,
	        TraceWriter writer)
		{
            await FunctionRunner.Run<PaymentEmployerAggregationFunction, EmployerTotalCostOfTraining>(writer,
                async (container, logger) =>
                {
	                var trainingCost = container.GetInstance<TrainingCost>();
	                await trainingCost.AddTotalCostOfTraining(employerTotalCostOfTraining);

                    return employerTotalCostOfTraining;
                });
        }
    }
}
