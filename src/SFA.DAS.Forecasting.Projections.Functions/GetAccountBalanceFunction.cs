using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class GetAccountBalanceFunction 
    {
        private readonly IGetAccountBalanceHandler _handler;

        public GetAccountBalanceFunction(IGetAccountBalanceHandler handler)
        {
            _handler = handler;
        }

        [FunctionName("GetAccountBalanceFunction")]
        [return: Queue(QueueNames.BuildProjections)]
        public async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.GetAccountBalance)]GenerateAccountProjectionCommand message,
            ILogger logger)
        {
            
            logger.LogDebug("Resolving GetAccountBalanceHandler from container.");
            
            await _handler.Handle(message);
            logger.LogInformation($"Finished generating the account projection in response to payment run: {message.EmployerAccountId}");
            return message;
            
        }
    }
}