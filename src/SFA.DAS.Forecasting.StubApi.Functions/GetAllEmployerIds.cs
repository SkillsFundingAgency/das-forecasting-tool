using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public class GetAllEmployerIds
    {
        private readonly IConfiguration _configuration;

        public GetAllEmployerIds(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [FunctionName("GetAllEmployerIds")]
        public async Task<IEnumerable<long>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employer/ids")]HttpRequestMessage req, 
            ILogger logger)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            if (_configuration["StubData"] != null && _configuration["StubData"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                return CreateFakeAccounts(_configuration["AccountStubLimit"]);
            }

            return
                StubDataStore.Apprenticeships
                    .Select(m => long.Parse(m.Key))
                    .Distinct();
        }

        private static List<long> CreateFakeAccounts(string accountStubLimit)
        {
            var numberOfAccounts = Convert.ToInt32(accountStubLimit);
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
