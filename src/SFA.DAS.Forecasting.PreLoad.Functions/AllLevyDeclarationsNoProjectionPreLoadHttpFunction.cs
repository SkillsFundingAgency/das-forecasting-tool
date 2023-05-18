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
    public class AllLevyDeclarationsNoProjectionPreLoadHttpFunction 
    {
        private readonly IEmployerDataService _employerDataService;

        public AllLevyDeclarationsNoProjectionPreLoadHttpFunction(IEmployerDataService employerDataService)
        {
            _employerDataService = employerDataService;
        }
        
        [FunctionName("AllLevyDeclarationsNoProjectionPreLoadHttpFunction")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "AllLevyDeclarationsNoProjectionPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.LevyPreLoadRequestNoProjection)] ICollector<PreLoadRequest> outputQueueMessage,
            ILogger logger)
        {
    
            var body = await req.Content.ReadAsStringAsync();
            var preLoadRequest = JsonConvert.DeserializeObject<PreLoadAllEmployersRequest>(body);

            
            var employerIds = await _employerDataService.EmployersForPeriod(preLoadRequest.PeriodYear, preLoadRequest.PeriodMonth);

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
