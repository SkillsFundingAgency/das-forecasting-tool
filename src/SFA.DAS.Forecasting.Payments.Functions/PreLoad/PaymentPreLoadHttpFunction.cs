using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Payments.Functions;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class PaymentPreLoadHttpFunction : IFunction
    {
        [FunctionName("PaymentPreLoadHttpFunction")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, 
            "post", Route = "PaymentPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            TraceWriter writer)
        {
            // Creates a msg for each EmployerAccountId
            await FunctionRunner.Run<PaymentPreLoadHttpFunction>(writer,
               async (container, logger) =>
               {
                   var body = await req.Content.ReadAsStringAsync();
                   var preLoadRequest = JsonConvert.DeserializeObject<PreLoadPaymentRequest>(body);

                   if (preLoadRequest == null)
                   {
                       logger.Warn($"{nameof(PreLoadPaymentRequest)} not valid. Function will exit.");
                       return;
                   }
                   logger.Info($"{nameof(PaymentPreLoadHttpFunction)} started. Data: {string.Join("|", preLoadRequest.EmployerAccountIds)}, {preLoadRequest.PeriodYear}, {preLoadRequest.PeriodMonth}");

                   var hashingService = container.GetInstance<IHashingService>();

                   foreach (var employerId in preLoadRequest.EmployerAccountIds)
                   {
                       var id = hashingService.DecodeValue(employerId);
                        outputQueueMessage.Add(
                            new PreLoadPaymentMessage
                            {
                                EmployerAccountId = id,
                                HashedEmployerAccountId = employerId,
                                PeriodYear = preLoadRequest.PeriodYear,
                                PeriodMonth = preLoadRequest.PeriodMonth,
                                PeriodId = preLoadRequest.PeriodId
                            }
                        );
                   }

                   logger.Info($"Added {preLoadRequest.EmployerAccountIds.Count()} levy declarations to {QueueNames.PreLoadPayment} queue.");
               });
        }
    }

    internal class PreLoadPaymentRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }

        public string PeriodId { get; set; }
    }
}
