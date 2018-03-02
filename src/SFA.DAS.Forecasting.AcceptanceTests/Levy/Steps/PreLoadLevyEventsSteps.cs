using System;
using Dapper;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Newtonsoft.Json;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;

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
            StartFunction("SFA.DAS.Forecasting.PreLoad.Functions");
            Thread.Sleep(1000);
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            ClearDatabase();
            var employerAccountId = "MJK9XV";
            var levyUrl =
                Config.ApiInsertLevyUrl.Replace("{employerAccountId}", employerAccountId);

            var client = new HttpClient();
            var levy = JsonConvert.SerializeObject(GetLevy(employerAccountId));
            await client.PostAsync(levyUrl, new StringContent(levy));
            Thread.Sleep(500);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Thread.Sleep(500);
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

        private static List<LevyDeclarationViewModel> GetLevy(string accountId)
        {
            return new List<LevyDeclarationViewModel>
            {
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "18-19",
                    PayrollMonth = 1,
                    CreatedDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 200
                },
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "18-19",
                    PayrollMonth = 2,
                    CreatedDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 300
                },
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "17-18",
                    PayrollMonth = 12,
                    CreatedDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 300
                }
            };
        }
    }
}
