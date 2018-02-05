using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Services;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Binding]
    public class ProcessLevyEventCI_528Steps : StepsBase
    {
        private AzureTableService _azureTableService;

        [Scope(Feature = "ProcessLevyEvent [CI-528]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
        }


        [OneTimeSetUp]
        public void BeforeScenario()
        {
            _azureTableService = new AzureTableService(Config.AzureStorageConnectionString, Config.LevyDeclarationsTable);
            _azureTableService.EnsureExists();
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
        }

        [OneTimeTearDown]
        public void AfterSecnario()
        {
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
            Thread.Sleep(1000);
        }

        [Given(@"that I'm the ESFA")]
        public void GivenThatIMTheESFA()
        {
            //just for show
        }

        [Given(@"I have credited levy to employer accounts")]
        public void GivenIHaveCreditedLevyToEmployerAccounts()
        {
            //just for show
        }

        [When(@"the employer services notifies the Forecasting service of the Levy Credits")]
        public async Task GivenLevyCreditEventsHaveBeenCreated()
        {
            await PostData(ValidData());
        }

        [When(@"the employer service notifies the Forecasting service of the invalid Levy Credits")]
         public async Task WhenThereIsMissingEventData()
        {
            await PostData(InvalidData());
        }

        [Then(@"there should be (.*) levy credit events stored")]
        public void ThenThereAreLevyCreditEventsStored(int expectedRecordsoBeSaved)
        {
            var _records = Do(() => _azureTableService?.GetRecords<LevySchemeDeclarationUpdatedMessage>(EmployerAccountId.ToString()), expectedRecordsoBeSaved, TimeSpan.FromMilliseconds(1000), 5);
            Assert.AreEqual(expectedRecordsoBeSaved, _records.Count(), message: $"Only {expectedRecordsoBeSaved} record should validate and be saved to the database");
        }

        [Then(@"all of the levy declarations stored should be correct")]
        public void ThenAllOfTheLevyDeclarationsStoredIsCorrect()
        {
            var _records = _azureTableService?.GetRecords<LevySchemeDeclarationUpdatedMessage>(EmployerAccountId.ToString())?.ToList();

            _records.Should().Contain(m => m.LevyDeclaredInMonth == 301);
            _records.Should().Contain(m => m.LevyDeclaredInMonth == 201);
            _records.Should().Contain(m => m.LevyDeclaredInMonth == 101);
        }

        [Then(@"all the event with invalid data is not stored")]
        public void ThenAllTheEventWithInvalidDataIsNotStored()
        {
            var _records = _azureTableService?.GetRecords<LevySchemeDeclarationUpdatedMessage>(EmployerAccountId.ToString());

            Assert.AreEqual(0, _records.Count(m => m.AccountId.ToString().EndsWith("2")));
        }


        private IEnumerable<string> ValidData()
        {
            return
                new List<LevySchemeDeclarationUpdatedMessage> {
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 101,
                        CreatedDate = DateTime.Now,
                        PayrollYear = "18-19",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    },
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 201,
                        CreatedDate = DateTime.Now.AddMonths(-12),
                        PayrollYear = "18-19",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    },
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 301,
                        CreatedDate = DateTime.Now.AddMonths(-15),
                        PayrollYear = "18-19",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    }
                }
                .Select(m => JsonConvert.SerializeObject(m));
        }

        private IEnumerable<string> InvalidData()
        {
            return
                new List<LevySchemeDeclarationUpdatedMessage> {
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 102,
                        CreatedDate = DateTime.Now,
                        PayrollYear = "17-18",
                        PayrollMonth = 1,
                        EmpRef = ""
                    },
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 202,
                        CreatedDate = DateTime.Now.AddMonths(-25).AddDays(-1),
                        PayrollYear = "16-17",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    },
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 303,
                        CreatedDate = DateTime.Now.AddMonths(-15),
                        PayrollYear = "01-01",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    },
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = 501,
                        CreatedDate = DateTime.Now.AddMonths(-2),
                        PayrollYear = "17-18",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    },
                    new LevySchemeDeclarationUpdatedMessage {
                        AccountId = EmployerAccountId,
                        LevyDeclaredInMonth = -10,
                        CreatedDate = DateTime.Now.AddMonths(-2),
                        PayrollYear = "18-19",
                        PayrollMonth = 1,
                        EmpRef = "Not sure"
                    }

                }
                .Select(m => JsonConvert.SerializeObject(m));
        }

        private async Task PostData(IEnumerable<string> events)
        {
            var client = new HttpClient();

            var url = Path.Combine(Config.FunctionBaseUrl, "LevyDeclarationEventHttpFunction");
            foreach (var item in events)
            {
                await client.PostAsync(url, new StringContent(item));
            }

            Thread.Sleep(2000);
        }

        public static IEnumerable<T> Do<T>(
            Func<IEnumerable<T>> action,
            int expectedCount,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                var a = action();
                if(a.Count() == expectedCount)
                {
                    return a;
                }
                Thread.Sleep(retryInterval);
            }
            return new List<T>();
        }
    }
}
