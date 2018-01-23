using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Forecasting.Levy.Application.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public static class LevyDeclarationEventServiceBusFunction
    {
        [FunctionName("LevyDeclarationEventServiceBusFunction")]
        [return: Queue(QueueNames.LevyDeclarationValidator)]
        public static LevyDeclarationEvent Run(
            [ServiceBusTrigger("LevyDeclaration", "mysubscription", AccessRights.Manage)]LevyDeclarationEvent levyEvent, 
            TraceWriter log)
        {
            return levyEvent;
        }
    }
}
