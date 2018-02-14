using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class LevyDeclarationPreLoadHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationPreLoadHttpFunction")]
        [return: Queue(QueueNames.ValidateDeclaration)]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, 
            "post", Route = "LevyDeclarationPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.ValidateDeclaration)] ICollector<LevySchemeDeclarationUpdatedMessage> outputQueueMessage,
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationPreLoadHttpFunction>(writer,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadRequest>(body);

                   var levyDataService = container.GetInstance<EmployerDataService>();

                   if (preLoadRequest == null)
                   {
                       logger.Warn($"{nameof(PreLoadRequest)} not valid. Function will exit.");
                       return;
                   }

                   logger.Info($"LevyDeclarationPreLoadHttpFunction started. Data: {string.Join("|", preLoadRequest.EmployerAccountIds)}, {preLoadRequest.PeriodYear}, {preLoadRequest.PeriodMonth}");

                   var messageCount = 0;
                   foreach (var employerId in preLoadRequest.EmployerAccountIds)
                   {
                       var model = await levyDataService.LevyForPeriod(employerId, preLoadRequest.PeriodYear, preLoadRequest.PeriodMonth);
                       if (model != null)
                       {
                           messageCount++;
                           outputQueueMessage.Add(model);
                       }
                   }

                   logger.Info($"Added {messageCount} levy declarations to  {QueueNames.ValidateDeclaration} queue.");
               });
        }
    }

    internal class PreLoadRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public string PeriodYear { get; set; }

        public short? PeriodMonth { get; set; }
    }
}
