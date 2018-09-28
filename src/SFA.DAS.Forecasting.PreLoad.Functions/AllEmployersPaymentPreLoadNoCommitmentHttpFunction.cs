using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployersPaymentPreLoadNoCommitmentHttpFunction : IFunction
    {
        [FunctionName("AllEmployersPaymentPreLoadNoCommitmentHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "AllEmployersPaymentPreLoadNoCommitmentHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPaymentNoCommitment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<AllEmployersPaymentPreLoadNoCommitmentHttpFunction, string>(writer, executionContext,
               async (container, logger) =>
               {
                   var employerData = container.GetInstance<IEmployerDatabaseService>();
                   var hashingService = container.GetInstance<IHashingService>();

                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<AllEmployersPreLoadPaymentRequest>(body);

                   var employerIds = await employerData.GetEmployersWithPayments(preLoadRequest.PeriodYear, preLoadRequest.PeriodMonth);

                   var messageCount = 0;
                   foreach (var accountId in employerIds)
                   {
                       messageCount++;
                       var hashedAccountId = hashingService.HashValue(accountId);
                       outputQueueMessage.Add(
                           new PreLoadPaymentMessage
                           {
                               EmployerAccountId = accountId,
                               HashedEmployerAccountId = hashedAccountId,
                               PeriodYear = preLoadRequest.PeriodYear,
                               PeriodMonth = preLoadRequest.PeriodMonth,
                               PeriodId = preLoadRequest.PeriodId,
                               SubstitutionId = null
                           }
                       );
                   }

                   var msg = $"Added {messageCount} message(s) to queue for year: {preLoadRequest.PeriodYear} and month: {preLoadRequest.PeriodMonth}";
                   logger.Info(msg);
                   return msg;
               });
        }
    }
}
