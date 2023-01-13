using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationAllowProjectionFunction
    {
        private readonly IAllowAccountProjectionsHandler _handler;

        public LevyDeclarationAllowProjectionFunction(IAllowAccountProjectionsHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("LevyDeclarationAllowProjectionFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.AllowProjection)]LevySchemeDeclarationUpdatedMessage message,
            ILogger logger)
        {
            logger.LogDebug("Getting levy declaration handler from container.");
            var allowProjections = await _handler.Allow(message);
            if (!allowProjections)
            {
                logger.LogDebug($"Cannot generate the projections, still handling levy declarations. Employer: {message.AccountId}");
                return null;
            }

            logger.LogInformation($"Now sending message to trigger the account projections for employer '{message.AccountId}'");
            return new GenerateAccountProjectionCommand
            {
                EmployerAccountId = message.AccountId,
                ProjectionSource = ProjectionSource.LevyDeclaration
            };
              
        }
    }
}