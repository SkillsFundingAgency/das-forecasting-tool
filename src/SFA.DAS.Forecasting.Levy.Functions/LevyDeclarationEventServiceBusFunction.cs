using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationEventServiceBusFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventServiceBusFunction")]
        [return: Queue(QueueNames.ValidateDeclaration)]
        public static async Task<LevySchemeDeclarationUpdatedMessage> Run(
            [ServiceBusTrigger("LevyPeriod", "mysubscription", AccessRights.Manage)]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationEventValidatorFunction, LevySchemeDeclarationUpdatedMessage>(writer,
                async (container, logger) =>
                {
                    logger.Info($"Added {nameof(LevySchemeDeclarationUpdatedMessage)} to queue: {QueueNames.ValidateDeclaration},  for EmployerAccountId: {levySchemeUpdatedMessage?.AccountId}");
                    return await Task.FromResult(levySchemeUpdatedMessage);
                });
        }
    }
}
