using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class PaymentPreLoadHttpFunction 
    {
        [FunctionName("PaymentPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "PaymentPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            ILogger logger)
        {
            // Creates a msg for each EmployerAccountId
            
           var body = await req.Content.ReadAsStringAsync();
           var preLoadRequest = JsonConvert.DeserializeObject<PreLoadPaymentRequest>(body);

           if (preLoadRequest == null)
           {
               logger.LogWarning($"{nameof(PreLoadPaymentRequest)} not valid. Function will exit.");
               return "";
           }

           logger.LogInformation($"{nameof(PaymentPreLoadHttpFunction)} started. Data: {string.Join("|", preLoadRequest.EmployerAccountIds)}, {preLoadRequest.PeriodYear}, {preLoadRequest.PeriodMonth}");

           if (preLoadRequest.SubstitutionId != null && preLoadRequest.EmployerAccountIds.Count() != 1)
           {
               var msg = $"If {nameof(preLoadRequest.SubstitutionId)} is provded there may only be 1 EmployerAccountId.";
               logger.LogWarning(msg);
               return msg;
           }

           
           foreach (var accountId in preLoadRequest.EmployerAccountIds)
           {
                outputQueueMessage.Add(
                    new PreLoadPaymentMessage
                    {
                        EmployerAccountId = accountId,
                        PeriodYear = preLoadRequest.PeriodYear,
                        PeriodMonth = preLoadRequest.PeriodMonth,
                        PeriodId = preLoadRequest.PeriodId,
                        SubstitutionId = preLoadRequest.SubstitutionId
                    }
                );
           }

           if (preLoadRequest.SubstitutionId.HasValue)
           {
               return $"HashedDubstitutionId: {preLoadRequest.SubstitutionId.Value}";
           }

           logger.LogInformation($"Added {preLoadRequest.EmployerAccountIds.Count()} get payment messages to {QueueNames.PreLoadPayment} queue.");
           return $"Message added: {preLoadRequest.EmployerAccountIds.Count()}";
               
        }
    }
}
