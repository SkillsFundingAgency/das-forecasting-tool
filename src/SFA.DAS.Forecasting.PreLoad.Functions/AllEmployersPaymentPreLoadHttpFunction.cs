using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployersPaymentPreLoadHttpFunction
    {
        private readonly IEmployerDatabaseService _employerDatabaseService;

        public AllEmployersPaymentPreLoadHttpFunction(IEmployerDatabaseService employerDatabaseService)
        {
            _employerDatabaseService = employerDatabaseService;
        }
        
        [FunctionName("AllEmployersPaymentPreLoadHttpFunction")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "AllEmployersPaymentPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            ILogger logger)
        {
        
           var body = await req.Content.ReadAsStringAsync();
           var preLoadRequest = JsonConvert.DeserializeObject<AllEmployersPreLoadPaymentRequest>(body);

           var employerIds = await _employerDatabaseService.GetAccountIds();

           var messageCount = 0;
           foreach (var accountId in employerIds)
           {
               messageCount++;
               //var hashedAccountId = hashingService.HashValue(accountId);
               outputQueueMessage.Add(
                   new PreLoadPaymentMessage
                   {
                       EmployerAccountId = accountId,
                       //HashedEmployerAccountId = hashedAccountId,
                       PeriodYear = preLoadRequest.PeriodYear,
                       PeriodMonth = preLoadRequest.PeriodMonth,
                       PeriodId = preLoadRequest.PeriodId,
                       SubstitutionId = null
                   }
               );
           }

           var msg = $"Added {messageCount} message(s) to queue for year: {preLoadRequest.PeriodYear} and month: {preLoadRequest.PeriodMonth}";
           logger.LogInformation(msg);
           return msg;
        }
    }
}
