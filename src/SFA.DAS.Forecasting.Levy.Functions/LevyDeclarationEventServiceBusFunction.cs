using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationEventServiceBusFunction 
    {
        [FunctionName("LevyDeclarationEventServiceBusFunction")]
        [return: Queue(QueueNames.ValidateDeclaration)]
        public async Task<LevySchemeDeclarationUpdatedMessage> Run(
            [ServiceBusTrigger("LevyPeriod", "mysubscription")]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage, 
            ILogger logger)
        {
            logger.LogInformation($"Added {nameof(LevySchemeDeclarationUpdatedMessage)} to queue: {QueueNames.ValidateDeclaration},  for EmployerAccountId: {levySchemeUpdatedMessage?.AccountId}");
            return await Task.FromResult(levySchemeUpdatedMessage);
        }
    }
}
