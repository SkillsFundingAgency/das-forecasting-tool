using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.PreLoad.Functions.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllLevyDeclarationsPreLoadHttpFunction
    {
        private readonly IEmployerDataService _employerDataService;

        public AllLevyDeclarationsPreLoadHttpFunction(IEmployerDataService employerDataService)
        {
            _employerDataService = employerDataService;
        }
        [FunctionName("AllLevyDeclarationsPreLoadHttpFunction")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "AllLevyDeclarationsPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.LevyPreLoadRequest)] ICollector<PreLoadRequest> outputQueueMessage,
            ExecutionContext executionContext,
            ILogger logger)
        {
            
            var body = await req.Content.ReadAsStringAsync();
            var preLoadRequest = JsonConvert.DeserializeObject<PreLoadAllEmployersRequest>(body);

            var employerIds = await _employerDataService.GetAllAccounts();

            logger.LogInformation($"Found {employerIds.Count} employer(s) for period; Year: {preLoadRequest.PeriodYear} Month: {preLoadRequest.PeriodMonth}");

            foreach(var id in employerIds)
            {
                outputQueueMessage.Add(
                    new PreLoadRequest
                    {
                        EmployerAccountIds = new[] { id },
                        PeriodYear = preLoadRequest.PeriodYear,
                        PeriodMonth = preLoadRequest.PeriodMonth,
                    });
            }

            return $"Created {employerIds.Count} {nameof(PreLoadRequest)} messages";
        
        }
    }
}
