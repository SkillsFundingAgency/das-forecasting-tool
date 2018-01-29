using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Services;
using SFA.DAS.Forecasting.Levy.Application.Messages;
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
        private const long EmployerAccountId = 1111;
        private AzureTableService _azureTableService;


        [BeforeScenario]
        public void BeforeScenario()
        {
            _azureTableService = new AzureTableService(Config.AzureStorageConnectionString, Config.LevyDeclarationsTable);
            _azureTableService.EnsureExcists();
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
        }

        [AfterScenario]
        public void AfterSecnario()
        {
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
            Thread.Sleep(1000);
        }
        
        [Given(@"levy credit events have been created")]
        public async Task GivenLevyCreditEventsHaveBeenCreated()
        {
            await PostData(ValidData());
        }

        [Given(@"events with invalid data have been created")]
        public async Task WhenThereIsMissingEventData()
        {
            await PostData(InvalidData());
        }

        [Then(@"there are (.*) levy credit events stored")]
        public void ThenThereAreLevyCreditEventsStored(int expectedRecordsoBeSaved)
        {
            var _records = Do(() => _azureTableService?.GetRecords(EmployerAccountId.ToString()), expectedRecordsoBeSaved, TimeSpan.FromMilliseconds(1000), 5);
            _records.Count().Should().Be(expectedRecordsoBeSaved, because: $"Only {expectedRecordsoBeSaved} record should validate and be saved to the database");
        }

        [Then(@"all of the data stored is correct")]
        public void ThenAllOfTheDataStoredIsCorrect()
        {
            var _records = _azureTableService?.GetRecords(EmployerAccountId.ToString())?.ToList();

            _records.Should().Contain(m => m.Amount == 301);
            _records.Should().Contain(m => m.Amount == 201);
            _records.Should().Contain(m => m.Amount == 101);
            //Assert.IsTrue(_records.SingleOrDefault(m => m.Amount == 301) != null);
            //Assert.IsTrue(_records.SingleOrDefault(m => m.Amount == 201) != null);
            //Assert.IsTrue(_records.SingleOrDefault(m => m.Amount == 101) != null);
        }

        [Then(@"the event with invalid data is not stored")]
        public void ThenTheEventIsNotStored()
        {
            var _records = _azureTableService?.GetRecords(EmployerAccountId.ToString());

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
                        PayrollDate = DateTime.Now,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 201,
                        TransactionDate = DateTime.Now.AddMonths(-12),
                        PayrollDate = DateTime.Now.AddMonths(-12),
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 301,
                        TransactionDate = DateTime.Now.AddMonths(-15),
                        PayrollDate = DateTime.Now.AddMonths(-15),
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
                        PayrollDate = DateTime.Now,
                        Scheme = ""
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 202,
                        TransactionDate = DateTime.Now.AddMonths(-25).AddDays(-1),
                        PayrollDate = DateTime.Now.AddMonths(-12),
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 303,
                        TransactionDate = DateTime.Now.AddMonths(-15),
                        PayrollDate = DateTime.MinValue,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 501,
                        TransactionDate = DateTime.Now.AddMonths(-2),
                        PayrollDate = DateTime.Now.AddMonths(-2),
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = -10,
                        TransactionDate = DateTime.Now.AddMonths(-2),
                        PayrollDate = DateTime.Now.AddMonths(-2),
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
