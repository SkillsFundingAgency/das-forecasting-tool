using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class LevyDeclarationNoProjectionPreLoadHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationNoProjectionPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "LevyDeclarationNoProjectionPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.LevyPreLoadRequestNoProjection)] ICollector<PreLoadLevyRequest> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationNoProjectionPreLoadHttpFunction, string>(writer, executionContext,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadLevyRequest>(body);

                   outputQueueMessage.Add(preLoadRequest);

                   var msg = $"Added {nameof(PreLoadLevyRequest)} for levy declaration";
                   logger.Info(msg);
                   return msg;
               });
        }
    }
}