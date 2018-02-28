using System;
using Dapper;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using SFA.DAS.Forecasting.Models.Levy;
using FluentAssertions;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Scope(Feature = "PreLoadLevyEvents")]
    [Binding]
    public class PreLoadLevyEventsSteps : StepsBase
    {
        private static long _accountId = 12345;

        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
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
            //ClearDatabase();
            Thread.Sleep(1000);
        }

        [Given(@"I trigger function for 3 employers to have their data loaded.")]
        public async Task ITriggerFunction()
        {
            var item = "{\"EmployerAccountIds\":[" + _accountId + "],\"PeriodYear\":\"18-19\",\"PeriodMonth\":1}";
            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionUrl, new StringContent(item));
        }

        [Given(@"I trigger PreLoadEvent function for some employers with a substitution id (.*)")]
        public async Task ITriggerFunctionWithSubstitutionId(long substitutionId)
        {
            var item = "{\"EmployerAccountIds\":[\"MJK9XV\"],\"PeriodYear\":\"18-19\",\"PeriodMonth\":1, \"SubstitutionId\": " + substitutionId + "}";
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
                count = Connection.ExecuteScalar<int>("select Count(*) from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);
                return count == expectedCount;
            }, $"Failed to find all the levy declarations. Found {count} expected {expectedCount} for {_accountId}");
        }


        [Then(@"there will be a levy declaration for the employer (.*) and no sensitive data will have been stored in the database")]
        public void ThereWillRecordInTheStorageWithNoSensitiveData(long substitutionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", _accountId, DbType.Int64);
            var declaration = Connection.ExecuteScalar<LevyDeclaration>("Select * from LevyDeclaration where EmployerAccountId = @employerAccountId"
                 , param: parameters, commandType: CommandType.Text);

            declaration.Should().NotBeNull();
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
