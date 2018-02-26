using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using System.Threading;
using System.IO;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Binding]
    public class ProcessLevyEventCI_528Steps : StepsBase
    {
        [Scope(Feature = "Process Levy Event [CI-528]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
        }

        [Given(@"I'm a levy paying employer")]
        public void GivenIMALevyPayingEmployer()
        {
            //just for show
        }

        [Given(@"the payroll period is")]
        public void GivenThePayrollPeriodIs(Table table)
        {
            PayrollPeriod = table.CreateInstance<PayrollPeriod>();
            Assert.IsNotNull(PayrollPeriod);
            Assert.IsFalse(string.IsNullOrWhiteSpace(PayrollPeriod.PayrollYear));
            Assert.AreNotEqual(0, PayrollPeriod.PayrollMonth);
        }

        [Given(@"I have no existing levy declarations for the payroll period")]
        public void GivenIHaveNoExistingLevyDeclarations()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            parameters.Add("@payrollYear", PayrollPeriod.PayrollYear, DbType.String);
            parameters.Add("@payrollMonth", PayrollPeriod.PayrollMonth, DbType.Byte);
            Connection.Execute("Delete from LevyDeclaration where employerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth",
                    parameters, commandType: CommandType.Text);
        }

        [Given(@"I have made the following levy declarations")]
        public void GivenIHaveMadeTheFollowingLevyDeclarations(Table table)
        {
            LevySubmissions = table.CreateSet<LevySubmission>().ToList();
            Assert.IsTrue(LevySubmissions.Any());
        }

        [Given(@"I made some invalid levy declarations")]
        public void GivenIMadeSomeInvalidLevyDeclarations()
        {
            LevySubmissions = new List<LevySubmission>
            {
                new LevySubmission
                {
                    CreatedDate = "0001/01/01 00:00",
                    Amount = 7000,
                    Scheme = "ABCD-EFG"
                },
                new LevySubmission
                {
                    CreatedDate = DateTime.Today.ToString(),
                    Amount = 0,
                    Scheme = string.Empty
                },
            };
        }

        [When(@"the SFA Employer HMRC Levy service notifies the Forecasting service of the levy declarations")]
        public void WhenTheSFAEmployerHMRCLevyServiceNotifiesTheForecastingServiceOfTheLevyDeclarations()
        {
            LevySubmissions.Select(levySubmission => new LevySchemeDeclarationUpdatedMessage
            {
                Id = 123456,
                AccountId = Config.EmployerAccountId,
                LevyDeclaredInMonth = levySubmission.Amount,
                PayrollMonth = PayrollPeriod.PayrollMonth,
                PayrollYear = PayrollPeriod.PayrollYear,
                CreatedDate = levySubmission.CreatedDateValue,
                EmpRef = levySubmission.Scheme
            })
            .ToList()
            .ForEach(levyEvent =>
            {
                var payload = levyEvent.ToJson();
                var url = Config.LevyFunctionUrl;
                Console.WriteLine($"Sending levy event to levy function: {url}, Payload: {payload}");
                var response = HttpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json")).Result;
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [When(@"the employer service notifies the Forecasting service of the invalid Levy Credits")]
        public void WhenThereIsMissingEventData()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the Forecasting Levy service should store the levy declarations")]
        public void ThenTheForecastingLevyServiceShouldStoreTheLevyDeclarations()
        {
            WaitForIt(() =>
            {
                foreach (var levySubmission in LevySubmissions)
                {
                    Console.WriteLine($"Looking for Levy Declaration. Employer Account Id: {Config.EmployerAccountId}, Scheme: {levySubmission.Scheme}, Payroll Year - Month: {PayrollPeriod.PayrollYear}, {PayrollPeriod.PayrollMonth}, Amount: {levySubmission.Amount}");
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                    parameters.Add("@payrollYear", PayrollPeriod.PayrollYear, DbType.String);
                    parameters.Add("@payrollMonth", PayrollPeriod.PayrollMonth, DbType.Byte);
                    parameters.Add("@scheme", levySubmission.Scheme, DbType.String);
                    parameters.Add("@amount", levySubmission.Amount, DbType.Decimal);
                    var count = Connection.ExecuteScalar<int>("Select Count(*) from LevyDeclaration where employerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth and [scheme] = @scheme and [LevyAmountDeclared] = @amount",
                        parameters, commandType: CommandType.Text);
                    return count == 1;
                }
                return false;
            }, "Failed to find all the levy declarations.");
        }

        [Then(@"the Forecasting Levy service should not store the levy declarations")]
        public void ThenTheForecastingLevyServiceShouldNotStoreTheLevyDeclarations()
        {
            Thread.Sleep(Config.TimeToWait);
            Console.WriteLine($"Looking for Levy Declaration. Employer Account Id: {Config.EmployerAccountId}, Payroll Year - Month: {PayrollPeriod.PayrollYear}, {PayrollPeriod.PayrollMonth}");
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            parameters.Add("@payrollYear", PayrollPeriod.PayrollYear, DbType.String);
            parameters.Add("@payrollMonth", PayrollPeriod.PayrollMonth, DbType.Byte);
            var count = Connection.ExecuteScalar<int>("Select Count(*) from LevyDeclaration where employerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth",
                parameters, commandType: CommandType.Text);
            Assert.AreEqual(0, count);
        }
    }
}
