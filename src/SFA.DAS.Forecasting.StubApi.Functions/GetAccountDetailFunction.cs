using System;
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
    public static class GetAccountDetailFunction
    {
        [FunctionName("GetAccountDetailFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}")]HttpRequestMessage req, long accountId,
            TraceWriter log)
        {
            var vm = new AccountDetailViewModel
            {
                AccountId = accountId,
                Balance = StubDataStore.Balance,
                DasAccountName = "Test Employer",
                DateRegistered = DateTime.Today,
                HashedAccountId = "MDDP87",
            };

            var accountDetailsJson = JsonConvert.SerializeObject(vm);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(accountDetailsJson, Encoding.UTF8, "application/json")
            };
        }
    }
}
