using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public class AddBalanceFunction
    {

        [FunctionName("AddBalanceFunction")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "data/balance")]HttpRequestMessage req, 
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var body = await req.Content.ReadAsStringAsync();
            var balance = JsonConvert.DeserializeObject<decimal>(body);

            StubDataStore.Balance = balance;
        } 
    }
}
