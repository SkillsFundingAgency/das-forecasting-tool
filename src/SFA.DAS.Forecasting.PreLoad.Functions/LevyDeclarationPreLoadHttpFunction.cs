using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.PreLoad.Functions.Models;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class LevyDeclarationPreLoadHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationPreLoadHttpFunction")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "LevyDeclarationPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.ValidateLevyDeclaration)] ICollector<LevySchemeDeclarationUpdatedMessage> outputQueueMessage, 
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationPreLoadHttpFunction>(writer, executionContext,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadRequest>(body);

                   if (preLoadRequest == null)
                   {
                       logger.Warn($"{nameof(PreLoadRequest)} not valid. Function will exit.");
                       return;
                   }

                   var levyDataService = container.GetInstance<IEmployerDataService>();

                   logger.Info($"LevyDeclarationPreLoadHttpFunction started. Data: {string.Join("|", preLoadRequest.EmployerAccountIds)}, {preLoadRequest.PeriodYear}, {preLoadRequest.PeriodMonth}");

                   var messageCount = 0;
                   foreach (var employerId in preLoadRequest.EmployerAccountIds)
                   {
                       var model = await levyDataService.LevyForPeriod(employerId, preLoadRequest.PeriodYear, preLoadRequest.PeriodMonth);
                       if (model == null)
                           continue;
                       messageCount++;
                       outputQueueMessage.Add(model);
                   }

                   logger.Info($"Added {messageCount} levy declarations to  {QueueNames.ValidateLevyDeclaration} queue.");
               });
        }
    }
}