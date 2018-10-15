using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class GetAllEmployerIds
    {
        [FunctionName("GetAllEmployerIds")]
        public static async Task<IEnumerable<long>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employer/ids")]HttpRequestMessage req, 
            TraceWriter writer)
        {
            writer.Info("C# HTTP trigger function processed a request.");

            if (ConfigurationManager.AppSettings["StubData"] != null && ConfigurationManager.AppSettings["StubData"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                return CreateFakeAccounts();
            }

            return
                StubDataStore.Apprenticeships
                    .Select(m => long.Parse(m.Key))
                    .Distinct();
        }

        private static List<long> CreateFakeAccounts()
        {
            var numberOfAccounts = Convert.ToInt32(ConfigurationManager.AppSettings["AccountStubLimit"]);
            var accountList = new List<long>();
            var randomNumber = new Random();


            for (var i = 0; i <= numberOfAccounts; i++)
            {
                accountList.Add(randomNumber.Next(555555,999999));
            }

            return accountList;
        }
    }
}
