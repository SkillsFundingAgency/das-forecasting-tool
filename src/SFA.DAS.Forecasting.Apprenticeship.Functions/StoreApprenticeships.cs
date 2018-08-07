using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions
{
    public class StoreApprenticeships : IFunction
    {
        [FunctionName("StoreApprenticeships")]
        public static void Run(
            [QueueTrigger(QueueNames.StoreApprenticeships)]ApprenticeshipMessage message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            string _type = message.FundingSource == Models.Payments.FundingSource.Levy ? "Levy" : "Transfer";
            writer.Info($"Storing {_type} apprentice: {message.ApprenticeName}");
        }
    }
}
