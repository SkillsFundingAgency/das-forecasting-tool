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
        [return: Queue(QueueNames.LevyDeclarationValidator)]
        public static async Task<LevyDeclarationEvent> Run(
            [ServiceBusTrigger("LevyPeriod", "mysubscription", AccessRights.Manage)]LevyDeclarationEvent levyEvent, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationEventValidatorFunction, LevyDeclarationEvent>(writer,
                async (container, logger) =>
                {
                    logger.Info($"Added {nameof(LevyDeclarationEvent)} to queue: {QueueNames.LevyDeclarationValidator},  for EmployerAccountId: {levyEvent?.EmployerAccountId}");
                    return await Task.FromResult(levyEvent);
                });
        }
    }
}
