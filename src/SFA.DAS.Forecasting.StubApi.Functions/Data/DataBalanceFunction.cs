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
    public static class DataBalanceFunction
    {
        public static decimal Balance { get; set; }

        [FunctionName("DataBalancePostFunction")]
        public static async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "data/balance")]HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var body = await req.Content.ReadAsStringAsync();
            var balance = JsonConvert.DeserializeObject<decimal>(body);

            Balance = balance;
        } 
    }

    public static class DataBalanceFunctionGet
    {
        [FunctionName("DataBalanceGetFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}")]HttpRequestMessage req, long accountId,
            TraceWriter log)
        {
            var vm = new AccountDetailViewModel
            {
                AccountId = accountId,
                Balance = DataBalanceFunction.Balance,
                DasAccountName = "Test Employer",
                DateRegistered = DateTime.Today,
                HashedAccountId = "MDDP87",
            };

            var d = JsonConvert.SerializeObject(vm);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(d, Encoding.UTF8, "application/json")
            };
        }
    }
}
