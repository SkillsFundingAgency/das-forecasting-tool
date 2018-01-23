using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Levy.Application.Messages;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationEventHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventHttpFunction")]
        [return: Queue(QueueNames.LevyDeclarationValidator)]
        public static async Task<LevyDeclarationEvent> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LevyDeclarationEventHttpFunction")]HttpRequestMessage req, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationEventHttpFunction, LevyDeclarationEvent>(writer,
                async (container, logger) =>
                {
                    dynamic body = await req.Content.ReadAsStringAsync();
                    var levyDeclarationEvent = JsonConvert.DeserializeObject<LevyDeclarationEvent>(body as string);

                    logger.Info($"Added one levy declaration to {QueueNames.LevyDeclarationValidator} ");
                    return await Task.FromResult(levyDeclarationEvent);
                });
        }
    }
}
