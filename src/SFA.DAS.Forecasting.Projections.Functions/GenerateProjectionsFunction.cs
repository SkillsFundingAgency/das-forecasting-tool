using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;


namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class GenerateProjectionsFunction : IFunction
    {
        [FunctionName("GenerateProjectionsFunction")]
        [return:Queue(QueueNames.GetAccountBalance)]
        public static GenerateAccountProjectionCommand Run(
            [QueueTrigger(QueueNames.GenerateProjections)]GenerateAccountProjectionCommand message,
            TraceWriter writer)
        {
            return message;
        }
    }
}