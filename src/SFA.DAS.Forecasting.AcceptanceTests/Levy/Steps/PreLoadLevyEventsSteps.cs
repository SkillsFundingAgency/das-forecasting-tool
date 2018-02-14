using Dapper;
using SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Scope(Feature = FeatureName)]
    [Binding]
    public class PreLoadLevyEventsSteps : StepsBase
    {
        private const string FeatureName= "PreLoadLevyEvents";
        private static string Url = Path.Combine(Config.LevyFunctionUrl, "LevyDeclarationPreLoadHttpFunction");

        private static ApiHost _apiHost;
        
        private static IEnumerable<long> _accountIds = new List<long> { 497, 498, 499 };

        [Scope(Feature = FeatureName)]
        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            _apiHost = new ApiHost();
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
            Thread.Sleep(1000);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            ClearDatabase();
            Thread.Sleep(1000);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            //ClearDatabase();
            Thread.Sleep(1000);
        }

        [Given(@"I trigger function for 3 employers to have their data loaded.")]
        public async Task ITriggerFunction()
        {
            var item = "{\"EmployerAccountIds\":[\"MJK9XV\",\"MGXLRV\",\"MPN4YM\"],\"PeriodYear\":\"18-19\",\"PeriodMonth\":1}";

            var client = new HttpClient();
            await client.PostAsync(Url, new StringContent(item));
        }

        [When(@"data have been processed")]
        public void DataHaveBeenProcessed()
        {
            Thread.Sleep(1000);
        }

        [Then(@"there will be (.*) records in the storage")]
        public void ThereWillBeThreeRecordsInTheStorage(int expectedCount)
        {
            WaitForIt(() =>
            {
                foreach (var accountId in _accountIds)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", accountId, DbType.Int64);
                    var count = Connection.ExecuteScalar<int>("Select Count(*) from LevyDeclaration where EmployerAccountId = @employerAccountId"
                         , param: parameters, commandType: CommandType.Text);
                    return count == 1;
                }
                return false;
            }, "Failed to find all the levy declarations.");
        }

        private void ClearDatabase()
        {
            foreach (var accountId in _accountIds)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", accountId, DbType.Int64);
                var count = Connection.ExecuteScalar<int>("DELETE LevyDeclaration WHERE EmployerAccountId = @employerAccountId"
                     , param: parameters, commandType: CommandType.Text);
                
            }
        }
    }
}
