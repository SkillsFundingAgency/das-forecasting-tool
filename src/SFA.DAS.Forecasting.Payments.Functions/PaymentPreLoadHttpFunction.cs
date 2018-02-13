using System.Collections.Generic;
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
        [return: Queue(QueueNames.PreLoadPayment)]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, 
            "post", 
            Route = "PaymentPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            TraceWriter writer)
        {
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

                   var test8509 = hashingService.HashValue(8509);
                   var test1234 = hashingService.HashValue(1234);

                   var messageCount = 0;
                   foreach (var employerId in preLoadRequest.EmployerAccountIds)
                   {
                       var id = hashingService.DecodeValue(employerId);
                        messageCount++;
                        outputQueueMessage.Add(
                            new PreLoadPaymentMessage
                            {
                                EmployerAccountId = id,
                                PeriodYear = preLoadRequest.PeriodYear,
                                PeriodMonth = preLoadRequest.PeriodMonth
                            }
                        );
                   }

                   logger.Info($"Added {messageCount} levy declarations to  {QueueNames.PaymentValidator} queue.");
               });
        }
    }

    internal class PreLoadPaymentRequest
    {
        public IEnumerable<string> EmployerAccountIds { get; set; }

        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }
    }
}
