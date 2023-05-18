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
    public class AddLevyFunction
    {
        [FunctionName("AddLevyFunction")]
        public async Task<HttpResponseMessage> Run([
            HttpTrigger(AuthorizationLevel.Function, "post", 
            Route = "data/accounts/{accountId}/levy")]HttpRequestMessage req, string accountId, ILogger log)
        {
            log.LogInformation($"C# HTTP trigger for {nameof(AddLevyFunction)} POST.");

            var body = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<IEnumerable<LevyDeclarationViewModel>>(body);

            if (StubDataStore.LevyData.ContainsKey(accountId))
            {
                StubDataStore.LevyData.Remove(accountId);
            }

            StubDataStore.LevyData.Add(accountId, data);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"Data added for {accountId}", System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
