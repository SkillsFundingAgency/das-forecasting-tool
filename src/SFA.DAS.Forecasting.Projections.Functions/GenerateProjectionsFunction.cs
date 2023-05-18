using Microsoft.Azure.WebJobs;
using SFA.DAS.Forecasting.Messages.Projections;


namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class GenerateProjectionsFunction 
    {
        [FunctionName("GenerateProjectionsFunction")]
        [return:Queue(QueueNames.GetAccountBalance)]
        public GenerateAccountProjectionCommand Run([QueueTrigger(QueueNames.GenerateProjections)]GenerateAccountProjectionCommand message)
        {
            return message;
        }
    }
}