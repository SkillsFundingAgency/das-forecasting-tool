using System;
using Dapper;
using System.Data;
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

        private static long _accountId = 497;

        [Scope(Feature = FeatureName)]
        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
            Thread.Sleep(1000);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            ClearDatabase();
            Thread.Sleep(500);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Thread.Sleep(1000);
        }

        [Given(@"I trigger function for 3 employers to have their data loaded.")]
        public async Task ITriggerFunction()
        {
            var item = "{\"EmployerAccountIds\":[\"MJK9XV\",\"MGXLRV\",\"MPN4YM\"],\"PeriodYear\":\"18-19\",\"PeriodMonth\":2}";
            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionUrl, new StringContent(item));
        }

        [When(@"data have been processed")]
        public void DataHaveBeenProcessed()
        {
            Thread.Sleep(1000);
        }

        [Then(@"there will be (.*) records in the storage")]
        public void ThereWillBeThreeRecordsInTheStorage(int expectedCount)
        {
            var count = 0;
            WaitForIt(() =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", _accountId, DbType.Int64);
                count = Connection.ExecuteScalar<int>("Select Count(*) from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);
                return count == 1;
            }, $"Failed to find all the levy declarations. Found {count} for {_accountId}");
        }

        private void ClearDatabase()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", _accountId, DbType.Int64);
            var count = Connection.ExecuteScalar<int>("DELETE LevyDeclaration WHERE EmployerAccountId = @employerAccountId"
                    , param: parameters, commandType: CommandType.Text);
        }
    }
}
