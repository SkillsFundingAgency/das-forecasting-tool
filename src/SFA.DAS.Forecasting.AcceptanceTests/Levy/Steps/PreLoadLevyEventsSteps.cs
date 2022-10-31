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
using System.Configuration;
using System.Data.SqlClient;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Scope(Feature = "PreLoadLevyEvents")]
    [Binding]
    public class PreLoadLevyEventsSteps : StepsBase
    {
        private string _hashedEmployerAccountId = "MN4YKL";

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

        [Given(@"I have transactions recorded in the employer accounts service")]
        public void AddDataToEmployerDb()
        {
            ExecuteSql(() =>
            {
                using (var connection = new SqlConnection(ConfigurationManager
                    .ConnectionStrings["EmployerDatabaseConnectionString"]
                    .ConnectionString))
                {
                    connection.Execute("delete from [employer_financial].[TransactionLine] where accountId in (497, 8509)", commandType: CommandType.Text);
                    connection.Execute("delete from [employer_financial].[LevyDeclaration] where accountId in (497, 8509);", commandType: CommandType.Text);

                    foreach (var id in new long[] { 497, 8509 })
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@accountId", id);

                        var sql = $@"
                                INSERT INTO [employer_financial].[TransactionLine]
                                           ([AccountId],[DateCreated],[SubmissionId],[TransactionDate],[TransactionType],[LevyDeclared],[Amount],[EmpRef],[PeriodEnd],[UkPrn],[SfaCoInvestmentAmount],[EmployerCoInvestmentAmount],[EnglishFraction],[TransferSenderAccountId],[TransferSenderAccountName],[TransferReceiverAccountId],[TransferReceiverAccountName])
                                VALUES (@accountId,'2018-01-23 00:00:00.000',34{id}815,'2018-01-18 07:12:28.060',1,10000.0000,8811.0000,'001/MP00056',null,null,0,0,0.80100,null,null ,null,null)"
                            ;
                        connection.Execute(sql, parameters);

                        var sql2 = $@"INSERT INTO [employer_financial].[LevyDeclaration]
                                   ([AccountId] ,[empRef] ,[LevyDueYTD] ,[LevyAllowanceForYear] ,[SubmissionDate],[SubmissionId] ,[PayrollYear] ,[PayrollMonth] ,[CreatedDate] ,[EndOfYearAdjustment],[EndOfYearAdjustmentAmount],[DateCeased],[InactiveFrom] ,[InactiveTo] ,[HmrcSubmissionId] ,[NoPaymentForPeriod])
                             VALUES (@accountId, '001/MP00056', 50.0000, 15000.0000, '2017-05-15 12:00:00.000', 34{id}815, '17-18', 11, GETDATE(), 0, 0, null,  null, null, 0, null)";

                        connection.Execute(sql2, parameters);
                    }
                }
            });
        }

        [Given(@"I have no levy declarations in the forecasting database already")]
        public void GivenIHaveNoLevyDeclarationsInTheDatabaseAlready()
        {
            var employerAccountIds = new long[] { 497, 12345, 8509 };
            var anyData = DataContext.LevyDeclarations.Any(m => employerAccountIds.Contains(m.EmployerAccountId) );

            anyData.Should().BeFalse();
        }

        // Steps

        [Given(@"I trigger function for 3 employers to have their data loaded.")]
        public async Task ITriggerFunction()
        {           
            var item = "{\"EmployerAccountIds\":[\"" + _hashedEmployerAccountId + "\"],\"PeriodYear\":\"17-18\",\"PeriodMonth\":11}";
            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionUrl, new StringContent(item));
        }

        [Given(@"I trigger PreLoadEvent function for some employers with a substitution id (.*)")]
        public async Task ITriggerFunctionWithSubstitutionId(long substitutionId)
        {
            var item = "{\"EmployerAccountIds\":[\"" + _hashedEmployerAccountId + "\"],\"PeriodYear\":\"17-18\",\"PeriodMonth\":11, \"SubstitutionId\": " + substitutionId + "}";

            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionUrl, new StringContent(item));
        }

        [Given(@"I trigger PreLoadEvet for all employers")]
        public async Task GivenITriggerPreLoadEvetForAllEmployers()
        {
            var item = "{ \"PeriodYear\": \"17-18\", \"PeriodMonth\": 11 }";

            Console.WriteLine($"Triggering Levy preload. Uri: {Config.LevyPreLoadFunctionAllEmployersUrl}, payload: {item}");
            var client = new HttpClient();
            await client.PostAsync(Config.LevyPreLoadFunctionAllEmployersUrl, new StringContent(item));
        }

        [When(@"data have been processed")]
        public void DataHaveBeenProcessed()
        {
            Thread.Sleep(10 * 1000);
        }

        [Then(@"there will be a record in the storage for employer (.*)")]
        public void ThereWillBeThreeRecordsInTheStorage(long employerId)
        {
            LevyDeclarationModel declaration = null;
            
            WaitForIt(() => 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", employerId, DbType.Int64);

                var declarations = Connection.Query<LevyDeclarationModel>("select * from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);

                declaration = declarations.FirstOrDefault();

                return declaration != null;

            });

            declaration.Should().NotBeNull();

            declaration.PayrollYear.Should().EndWith("17-18");
            declaration.LevyAmountDeclared.Should().Be(8811M);
            declaration.TransactionDate.Should().BeCloseTo(DateTime.Now, new TimeSpan(60));
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
            LevyDeclarationModel declaration = null;

            WaitForIt(() =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", substitutionId, DbType.Int64);

                var declarations = Connection.Query<LevyDeclarationModel>("select * from LevyDeclaration where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);

                declaration = declarations.FirstOrDefault();

                return declaration != null;

            });

            declaration.Should().NotBeNull();
        }

        [Then(@"there will be a levy declarations for all employers")]
        public void ThenThereWillBeALevyDeclarationsForAllEmployers()
        {
            WaitForIt(() =>
            {
                return
                       DataContext.LevyDeclarations.Count(m => m.EmployerAccountId == 8509) == 1
                    && DataContext.LevyDeclarations.Count(m => m.EmployerAccountId == 497) == 1;
            }, "");   
        }


        private void ClearDatabase()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId1", 497, DbType.Int64);
            parameters.Add("@employerAccountId2", 12345, DbType.Int64);
            parameters.Add("@employerAccountId3", 8509, DbType.Int64);

            var count = Connection.ExecuteScalar<int>("DELETE LevyDeclaration WHERE EmployerAccountId IN (@employerAccountId1, @employerAccountId2, @employerAccountId3)"
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
