using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventStoreFunction 
    {
        private readonly IStoreLevyDeclarationHandler _handler;

        public LevyDeclarationEventStoreFunction(IStoreLevyDeclarationHandler handler)
        {
            _handler = handler;
        }
        
        [FunctionName("LevyDeclarationEventStoreFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.StoreLevyDeclaration)]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage, 
            ILogger logger)
        {
            logger.LogDebug("Getting levy declaration handler from container.");
            
            await _handler.Handle(levySchemeUpdatedMessage, QueueNames.AllowProjection);
            logger.LogInformation($"Finished handling levy declaration for EmployerAccountId: {levySchemeUpdatedMessage.AccountId}, PayrollYear: {levySchemeUpdatedMessage.PayrollYear}, month: {levySchemeUpdatedMessage.PayrollMonth}, scheme: {levySchemeUpdatedMessage.EmpRef}");
            
        }
    }
}
