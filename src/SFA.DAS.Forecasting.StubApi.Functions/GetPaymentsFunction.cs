using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.StubApi.Functions
{

    public static class GetPaymentsFunction
    {
        [FunctionName("GetPaymentsFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payments")]HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info($"C# HTTP trigger function {nameof(GetPaymentsFunction)}.");

            var data = JsonConvert.SerializeObject(StubDataStore.PaymentsData);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
        }
    }
}