using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Events.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class PaymentPreLoadHttpFunction : IFunction
    {
        [FunctionName("PaymentPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "PaymentPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            // Creates a msg for each EmployerAccountId
            return await FunctionRunner.Run<PaymentPreLoadHttpFunction, string>(writer, executionContext,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadPaymentRequest>(body);

                   if (preLoadRequest == null)
                   {
                       logger.Warn($"{nameof(PreLoadPaymentRequest)} not valid. Function will exit.");
                       return "";
                   }

                   logger.Info($"{nameof(PaymentPreLoadHttpFunction)} started. Data: {string.Join("|", preLoadRequest.EmployerAccountIds)}, {preLoadRequest.PeriodYear}, {preLoadRequest.PeriodMonth}");

                   if (preLoadRequest.SubstitutionId != null && preLoadRequest.EmployerAccountIds.Count() != 1)
                   {
                       var msg = $"If {nameof(preLoadRequest.SubstitutionId)} is provded there may only be 1 EmployerAccountId.";
                       logger.Warn(msg);
                       return msg;
                   }

                   foreach (var employerId in preLoadRequest.EmployerAccountIds)
                   {
                        outputQueueMessage.Add(
                            new PreLoadPaymentMessage
                            {
                                EmployerAccountId = employerId,
                                PeriodYear = preLoadRequest.PeriodYear,
                                PeriodMonth = preLoadRequest.PeriodMonth,
                                PeriodId = preLoadRequest.PeriodId,
                                SubstitutionId = preLoadRequest.SubstitutionId
                            }
                        );
                   }

                   if (preLoadRequest.SubstitutionId.HasValue)
                   {
                       var hashingService = container.GetInstance<IHashingService>();
                       var hashedDubstitutionId = hashingService.HashValue(preLoadRequest.SubstitutionId.Value);
                       return $"HashedDubstitutionId: {hashedDubstitutionId}";
                   }

                   logger.Info($"Added {preLoadRequest.EmployerAccountIds.Count()} levy declarations to {QueueNames.PreLoadPayment} queue.");
                   return $"Message added: {preLoadRequest.EmployerAccountIds.Count()}";
               });
        }
    }
}
