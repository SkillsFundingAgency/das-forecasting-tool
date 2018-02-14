using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationEventHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventHttpFunction")]
        [return: Queue(QueueNames.ValidateDeclaration)]
        public static async Task<LevySchemeDeclarationUpdatedMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LevyDeclarationEventHttpFunction")]HttpRequestMessage req, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationEventHttpFunction, LevySchemeDeclarationUpdatedMessage>(writer,
                 async (container, logger) =>
                {
                    var body = await req.Content.ReadAsStringAsync();
                    var levySchemeUpdatedEvent = JsonConvert.DeserializeObject<LevySchemeDeclarationUpdatedMessage>(body);

                    logger.Debug($"Received levy scheme declaration event via http endpoint: {levySchemeUpdatedEvent.ToDebugJson()}");
                    return levySchemeUpdatedEvent;
                });
        }
    }   
}
