using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class BuildProjectionsFunction 
    {
        private readonly IBuildAccountProjectionHandler _handler;

        public BuildProjectionsFunction(IBuildAccountProjectionHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("BuildProjectionsFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.BuildProjections)]GenerateAccountProjectionCommand message,
            ILogger logger)
        {
            logger.LogDebug("Resolving BuildAccountProjectionHandler from container.");
            await _handler.Handle(message);
            logger.LogInformation($"Finished building the account projection for employer: {message.EmployerAccountId}");

        }
    }
}