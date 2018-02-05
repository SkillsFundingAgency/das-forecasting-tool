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


        [BeforeScenario]
        public void BeforeScenario()
        {
            _azureTableService = new AzureTableService(Config.AzureStorageConnectionString, Config.LevyDeclarationsTable);
            _azureTableService.EnsureExists();
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
        }

        [AfterScenario]
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
            var _records = Do(() => _azureTableService?.GetRecordsStartingWith<LevyDeclarationEvent>(EmployerAccountId.ToString()), expectedRecordsoBeSaved, TimeSpan.FromMilliseconds(1000), 5);
            Assert.AreEqual(expectedRecordsoBeSaved, _records.Count(), message: $"Only {expectedRecordsoBeSaved} record should validate and be saved to the database");
        }

        [Then(@"all of the levy declarations stored should be correct")]
        public void ThenAllOfTheLevyDeclarationsStoredIsCorrect()
        {
            var _records = _azureTableService?.GetRecordsStartingWith<LevyDeclarationEvent>(EmployerAccountId.ToString())?.ToList();

            _records.Should().Contain(m => m.Amount == 301);
            _records.Should().Contain(m => m.Amount == 201);
            _records.Should().Contain(m => m.Amount == 101);
        }

        [Then(@"all the event with invalid data is not stored")]
        public void ThenAllTheEventWithInvalidDataIsNotStored()
        {
            var _records = _azureTableService?.GetRecords<LevyDeclarationEvent>(EmployerAccountId.ToString());

            Assert.AreEqual(0, _records.Count(m => m.EmployerAccountId.ToString().EndsWith("2")));
        }


        private IEnumerable<string> ValidData()
        {
            return
                new List<LevyDeclarationEvent> {
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 101,
                        TransactionDate = DateTime.Now,
                        PayrollYear = "18-19",
                        PayrollMonth = 1,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 201,
                        TransactionDate = DateTime.Now.AddMonths(-12),
                        PayrollYear = "17-18",
                        PayrollMonth = 1,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 301,
                        TransactionDate = DateTime.Now.AddMonths(-15),
                        PayrollYear = "16-17",
                        PayrollMonth = 10,
                        Scheme = "Not sure"
                    }
                }
                .Select(m => JsonConvert.SerializeObject(m));
        }

        private IEnumerable<string> InvalidData()
        {
            return
                new List<LevyDeclarationEvent> {
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 102,
                        TransactionDate = DateTime.Now,
                        PayrollYear = "17-18",
                        PayrollMonth = 1,
                        Scheme = ""
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 202,
                        TransactionDate = DateTime.Now.AddMonths(-25).AddDays(-1),
                        PayrollYear = "16-17",
                        PayrollMonth = 1,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 303,
                        TransactionDate = DateTime.Now.AddMonths(-15),
                        PayrollYear = "01-01",
                        PayrollMonth = 1,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 501,
                        TransactionDate = DateTime.Now.AddMonths(-2),
                        PayrollYear = "17-18",
                        PayrollMonth = 1,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = -10,
                        TransactionDate = DateTime.Now.AddMonths(-2),
                        PayrollYear = "18-19",
                        PayrollMonth = 1,
                        Scheme = "Not sure"
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

        private static IEnumerable<T> Do<T>(
            Func<IEnumerable<T>> action,
            int expectedCount,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            IEnumerable<T> rList = null;
            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                var a = action();
                if(a?.Count() == expectedCount)
                {
                    return a;
                }
                rList = a;
                Thread.Sleep(retryInterval);
            }
            return rList ?? new List<T>();
        }
    }
}
