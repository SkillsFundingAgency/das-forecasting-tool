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
    public class LevyDeclarationNoProjectionEventHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationNoProjectionEventHttpFunction")]
        [return: Queue(QueueNames.ValidateLevyDeclarationNoProjection)]
        public static async Task<LevySchemeDeclarationUpdatedMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LevyDeclarationNoProjectionEventHttpFunction")]HttpRequestMessage req,
            TraceWriter writer, ExecutionContext executionContext)
        {
            return await FunctionRunner.Run<LevyDeclarationNoProjectionEventHttpFunction, LevySchemeDeclarationUpdatedMessage>(writer, executionContext,
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
