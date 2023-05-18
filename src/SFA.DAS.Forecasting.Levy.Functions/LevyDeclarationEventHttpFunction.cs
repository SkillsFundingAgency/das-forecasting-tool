using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Domain.Extensions;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationEventHttpFunction 
    {
        [FunctionName("LevyDeclarationEventHttpFunction")]
        [return: Queue(QueueNames.ValidateDeclaration)]
        public async Task<LevySchemeDeclarationUpdatedMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LevyDeclarationEventHttpFunction")]HttpRequestMessage req,
            ILogger logger)
        {
            
            var body = await req.Content.ReadAsStringAsync();
            var levySchemeUpdatedEvent = JsonConvert.DeserializeObject<LevySchemeDeclarationUpdatedMessage>(body);

            logger.LogDebug($"Received levy scheme declaration event via http endpoint: {levySchemeUpdatedEvent.ToDebugJson()}");
            return levySchemeUpdatedEvent;
                
        }
    }
}
