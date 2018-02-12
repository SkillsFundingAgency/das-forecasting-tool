﻿using Newtonsoft.Json;
using SFA.DAS.Forecasting.AcceptanceTests.Services;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Core;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Binding]
    public class ProcessLevyEventCI_528Steps : StepsBase
    {
        private AzureTableService _azureTableService;
        protected List<LevySubmission> LevySubmissions { get => Get<List<LevySubmission>>(); set => Set(value); }
        [Scope(Feature = "Process Levy Event [CI-528]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
        }

<<<<<<< HEAD
        [Scope(Feature = "ProcessLevyEvent [CI-528]")]
        [BeforeScenario]
        public void BeforeScenario()
        {
            _azureTableService = new AzureTableService(Config.AzureStorageConnectionString, Config.LevyDeclarationsTable);
            _azureTableService.EnsureExists();
            _azureTableService.DeleteEntitiesStartingWith(EmployerAccountId.ToString());
        }

        [Scope(Feature = "ProcessLevyEvent [CI-528]")]
        [AfterScenario]
        public void AfterScenario()
        {
            _azureTableService.DeleteEntitiesStartingWith(EmployerAccountId.ToString());
            Thread.Sleep(1000);
        }

        [Given(@"that I'm the ESFA")]
        public void GivenThatIMTheESFA()
=======
        [Given(@"I'm a levy paying employer")]
        public void GivenIMALevyPayingEmployer()
>>>>>>> master
        {
            //just for show
        }

        [Given(@"the payroll period is")]
        public void GivenThePayrollPeriodIs(Table table)
        {
            PayrollPeriod = table.CreateInstance<PayrollPeriod>();
        }

        [Given(@"I have no existing levy declarations for the payroll period")]
        public void GivenIHaveNoExistingLevyDeclarations()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            parameters.Add("@payrollYear", PayrollPeriod.Year, DbType.String);
            parameters.Add("@payrollMonth", PayrollPeriod.Month, DbType.Byte);
            Connection.Execute("Delete from LevyDeclaration where employerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth",
                    parameters, commandType: CommandType.Text);
        }

        [Given(@"I have made the following levy declarations")]
        public void GivenIHaveMadeTheFollowingLevyDeclarations(Table table)
        {
            LevySubmissions = table.CreateSet<LevySubmission>().ToList();
            Assert.IsTrue(LevySubmissions.Any());
        }

        [When(@"the SFA Employer HMRC Levy service notifies the Forecasting service of the levy declarations")]
        public void WhenTheSFAEmployerHMRCLevyServiceNotifiesTheForecastingServiceOfTheLevyDeclarations()
        {
            LevySubmissions.Select(levySubmission => new LevySchemeDeclarationUpdatedMessage
            {
                Id = 123456,
                AccountId = Config.EmployerAccountId,
                LevyDeclaredInMonth = levySubmission.Amount,
                PayrollMonth = PayrollPeriod.Month,
                PayrollYear = PayrollPeriod.Year,
                CreatedDate = levySubmission.CreatedDateValue,
                EmpRef = levySubmission.Scheme
            })
                .ToList()
                .ForEach(levyEvent =>
                {
                    var payload = levyEvent.ToJson();
                    Console.WriteLine($"Sending levy event to levy function: {Config.LevyFunctionUrl}, Payload: {payload}");
                    var response = HttpClient.PostAsync(Config.LevyFunctionUrl, new StringContent(payload)).Result;
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
                    Console.WriteLine($"Looking for Levy Declaration. Employer Account Id: {Config.EmployerAccountId}, Scheme: {levySubmission.Scheme}, Payroll Year - Month: {PayrollPeriod.Year}, {PayrollPeriod.Month}, Amount: {levySubmission.Amount}");
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                    parameters.Add("@payrollYear", PayrollPeriod.Year, DbType.String);
                    parameters.Add("@payrollMonth", PayrollPeriod.Month, DbType.Byte);
                    parameters.Add("@scheme", levySubmission.Scheme, DbType.String);
                    parameters.Add("@amount", levySubmission.Amount, DbType.Decimal);
                    var count = Connection.ExecuteScalar<int>("Select Count(*) from LevyDeclaration where employerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth and [scheme] = @scheme and [LevyAmountDeclared] = @amount",
                        parameters, commandType: CommandType.Text);
                    return count == 1;
                }
                return false;
            },"Failed to find all the levy declarations.");
        }

    }
}
