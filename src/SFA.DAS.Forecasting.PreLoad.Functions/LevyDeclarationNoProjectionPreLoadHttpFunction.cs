using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.PreLoad.Functions.Models;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class LevyDeclarationNoProjectionPreLoadHttpFunction 
    {
        [FunctionName("LevyDeclarationNoProjectionPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "LevyDeclarationNoProjectionPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.LevyPreLoadRequestNoProjection)] ICollector<PreLoadRequest> outputQueueMessage,
            ILogger logger)
        {
            
           var body = await req.Content.ReadAsStringAsync();
           var preLoadRequest = JsonConvert.DeserializeObject<PreLoadRequest>(body);

           outputQueueMessage.Add(preLoadRequest);

           var msg = $"Added {nameof(PreLoadRequest)} for levy declaration";
           logger.LogInformation(msg);
           return msg;
    
        }
    }
}