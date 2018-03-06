using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class LevyDeclarationPreLoadHttpFunction : IFunction
    {
        [FunctionName("LevyDeclarationPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "LevyDeclarationPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.ValidateLevyDeclaration)] ICollector<LevySchemeDeclarationUpdatedMessage> outputQueueMessage, 
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationPreLoadHttpFunction, string>(writer, executionContext,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadRequest>(body);
                   var hashingService = container.GetInstance<IHashingService>();

                   if (preLoadRequest == null)
                   {
                       var msg = $"{nameof(PreLoadRequest)} not valid. Function will exit.";
                       logger.Warn(msg);
                       return msg;
                   }

                   if (preLoadRequest.SubstitutionId.HasValue && preLoadRequest.EmployerAccountIds.Count() != 1)
                   {
                       var msg = $"If {nameof(preLoadRequest.SubstitutionId)} is provided there must be exactly 1 EmployerAccountId";
                       logger.Warn(msg);
                       return msg;
                   }

                   var levyDataService = container.GetInstance<IEmployerDataService>();

                   logger.Info($"LevyDeclarationPreLoadHttpFunction started. Data: {string.Join("|", preLoadRequest.EmployerAccountIds)}, {preLoadRequest.PeriodYear}, {preLoadRequest.PeriodMonth}");

                   var messageCount = 0;
                   foreach (var employerId in preLoadRequest.EmployerAccountIds)
                   {
                       var model = await levyDataService.LevyForPeriod(employerId, preLoadRequest.PeriodYear, preLoadRequest.PeriodMonth);
                       if (model == null)
                           continue;

                       if (preLoadRequest.SubstitutionId.HasValue)
                       {
                           model.AccountId = preLoadRequest.SubstitutionId.Value;
                           model.EmpRef = hashingService.HashValue(preLoadRequest.SubstitutionId.Value);
                       }

                       messageCount++;
                       outputQueueMessage.Add(model);
                   }

                   logger.Info($"Added {messageCount} levy declarations to  {QueueNames.ValidateLevyDeclaration} queue.");

                   if (preLoadRequest.SubstitutionId.HasValue)
                   {
                       return $"{hashingService.HashValue(preLoadRequest.SubstitutionId.Value)}";
                   }
                   return $"Added {messageCount} levy declarations";
               });
        }
    }

    internal class PreLoadRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public string PeriodYear { get; set; }

        public short? PeriodMonth { get; set; }

        public long? SubstitutionId { get; set; }
    }
}