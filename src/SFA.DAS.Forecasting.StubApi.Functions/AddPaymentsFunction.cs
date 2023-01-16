using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public class AddPaymentsFunction
    {
        [FunctionName("AddPaymentsFunction")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "data/payments")]HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function {nameof(AddPaymentsFunction)}.");

            var body = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PageOfResults<Payment>>(body);

            StubDataStore.PaymentsData = data;
        }
    }
}