using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class AddBalanceFunction
    {

        [FunctionName("AddBalanceFunction")]
        public static async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "data/balance")]HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var body = await req.Content.ReadAsStringAsync();
            var balance = JsonConvert.DeserializeObject<decimal>(body);

            StubDataStore.Balance = balance;
        } 
    }
}
