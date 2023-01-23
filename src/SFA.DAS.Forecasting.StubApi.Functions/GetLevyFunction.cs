using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public class GetLevyFunction
    {
        [FunctionName("GetLevyFunction")]
        public async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "accounts/{accountId}/levy")]HttpRequestMessage req, string accountId, ILogger log)
        {
            log.LogInformation($"C# HTTP trigger for {nameof(GetLevyFunction)} GET.");

            IEnumerable<LevyDeclarationViewModel> data;
            StubDataStore.LevyData.TryGetValue(accountId, out data);

            var d = JsonConvert.SerializeObject(data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(d, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}