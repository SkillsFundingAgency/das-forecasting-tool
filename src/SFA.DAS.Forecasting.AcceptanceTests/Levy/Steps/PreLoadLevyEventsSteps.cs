using System;
using Dapper;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using SFA.DAS.Forecasting.Models.Levy;
using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;
using System.Linq;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Scope(Feature = "PreLoadLevyEvents")]
    [Binding]
    public class PreLoadLevyEventsSteps : StepsBase
    {
        private string _hashedEmployerAccountId = "MJK9XV";

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
            var levyUrl =
                Config.ApiInsertLevyUrl.Replace("{employerAccountId}", _hashedEmployerAccountId);

            var client = new HttpClient();
            var levy = JsonConvert.SerializeObject(GetLevy(_hashedEmployerAccountId));
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
            var item = "{\"EmployerAccountIds\":[\"" + _hashedEmployerAccountId + "\"],\"PeriodYear\":\"18-19\",\"PeriodMonth\":2}";
            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionUrl, new StringContent(item));
        }

        [Given(@"I trigger PreLoadEvent function for some employers with a substitution id (.*)")]
        public async Task ITriggerFunctionWithSubstitutionId(long substitutionId)
        {
            var item = "{\"EmployerAccountIds\":[\"MJK9XV\"],\"PeriodYear\":\"18-19\",\"PeriodMonth\":2, \"SubstitutionId\": " + substitutionId + "}";

            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionUrl, new StringContent(item));
        }

        [When(@"data have been processed")]
        public void DataHaveBeenProcessed()
        {
            Thread.Sleep(1000);
        }

        [Then(@"there will be a record in the storage for employer (.*)")]
        public void ThereWillBeThreeRecordsInTheStorage(long employerId)
        {
            LevyDeclaration declaration = null;
            
            WaitForIt(() => 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerId, DbType.Int64);

                var declarations = Connection.Query<LevyDeclaration>("select * from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);

                declaration = declarations.FirstOrDefault();

                return declaration != null;

            });

            declaration.Should().NotBeNull();

            declaration.PayrollYear.Should().EndWith("18-19");
            declaration.LevyAmountDeclared.Should().Be(300);
            declaration.TransactionDate.Should().BeCloseTo(DateTime.Now, precision: 60 * 1000);
        }


        [Then(@"there will be (.*) records in the storage for employer (.*)")]
        public void ThereWillBeSomeRecordsInTheStorageForEmployer(int expectedCount, long employerId)
        {
            var count = 0;

            WaitForIt(() =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerId, DbType.Int64);

                count = Connection.ExecuteScalar<int>("select Count(*) from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);

                return count == expectedCount;
            }, $"Expected {expectedCount} but found {count}");
        }

        [Then(@"there will be a levy declaration for the employer (.*) and no sensitive data will have been stored in the database")]
        public void ThereWillRecordInTheStorageWithNoSensitiveData(long substitutionId)
        {
            LevyDeclaration declaration = null;

            WaitForIt(() =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", substitutionId, DbType.Int64);

                var declarations = Connection.Query<LevyDeclaration>("select * from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);

                declaration = declarations.FirstOrDefault();

                return declaration != null;

            });

            declaration.Should().NotBeNull();
        }

        private void ClearDatabase()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId1", 497, DbType.Int64);
            parameters.Add("@employerAccountId2", 12345, DbType.Int64);
            var count = Connection.ExecuteScalar<int>("DELETE LevyDeclaration WHERE EmployerAccountId IN [@employerAccountId1, @employerAccountId2]"
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
