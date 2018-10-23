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
    public class LevyDeclarationPreLoadHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LevyDeclarationPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.LevyPreLoadRequest)] ICollector<PreLoadLevyMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationPreLoadHttpFunction, string>(writer, executionContext,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadLevyRequest>(body);

                   foreach (var accountId in preLoadRequest.EmployerAccountIds)
                   {
                       outputQueueMessage.Add(new PreLoadLevyMessage
                       {
                           EmployerAccountId = accountId,
                           PeriodMonth = preLoadRequest.PeriodMonth,
                           PeriodYear = preLoadRequest.PeriodYear,
                           SubstitutionId = preLoadRequest.SubstitutionId
                       });
                   }

                   var msg = $"Added {nameof(PreLoadLevyMessage)} for levy declaration";
                   logger.Info(msg);
                   return msg;
               });
        }
    }
}