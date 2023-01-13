using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventNoProjectionStoreFunction
    {
        private readonly IStoreLevyDeclarationHandler _handler;

        public LevyDeclarationEventNoProjectionStoreFunction(IStoreLevyDeclarationHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("LevyDeclarationEventNoProjectionStoreFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.StoreLevyDeclarationNoProjection)]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage,
            ILogger logger)
        {
            
            logger.LogDebug("Getting levy declaration handler from container.");
            
            await _handler.Handle(levySchemeUpdatedMessage, string.Empty);
            logger.LogDebug($"Finished handling past levy declaration for EmployerAccountId: {levySchemeUpdatedMessage.AccountId}, PayrollYear: {levySchemeUpdatedMessage.PayrollYear}, month: {levySchemeUpdatedMessage.PayrollMonth}, scheme: {levySchemeUpdatedMessage.EmpRef}");
        }
    }
}
