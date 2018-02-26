using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class GetLevyFunction
    {
        [FunctionName("GetLevyFunction")]
        public static async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "accounts/{accountId}/levy")]HttpRequestMessage req, string accountId, TraceWriter log)
        {
            log.Info($"C# HTTP trigger for {nameof(GetLevyFunction)} GET.");

            IEnumerable<LevyDeclarationViewModel> data;
            StubDataStore.LevyData.TryGetValue(accountId, out data);

            var d = JsonConvert.SerializeObject(data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(d, Encoding.UTF8, "application/json")
            };
        }
    }
}