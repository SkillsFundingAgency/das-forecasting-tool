using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Services;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Binding]
    public class ProcessLevyEventCI_528Steps
    {
        // ToDo: Move to config...
        private const string TableName = "LevyDeclarations";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private const long EmployerAccountId = 1111;
        private string _baseUrl = "http://localhost:7071/api";

        private AzureTableService _azureTableService;
        private List<LevyDeclarationEvent> _records;


        [BeforeScenario]
        public void BeforeScenario()
        {
            _azureTableService = new AzureTableService(ConnectionString, TableName);
            _azureTableService.EnsureExcists();
        }

        [AfterScenario]
        public void AfterSecnario()
        {
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
        }

        [Given(@"that I'm the ESFA")]
        public void GivenThatIMTheESFA()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Given(@"I have credited levy to employer accouns")]
        public void GivenIHaveCreditedLevyToEmployerAccouns()
        {
            // ScenarioContext.Current.Pending();
        }
        
        [Given(@"levy credit events have been created")]
        public async Task GivenLevyCreditEventsHaveBeenCreated()
        {

            var client = new HttpClient();

            foreach (var item in Data())
            {
                await client.PostAsync($"{_baseUrl}/LevyDeclarationEventHttpFunction", new StringContent(item));
                Thread.Sleep(100);
            }
        }
        
        [Then(@"the data for each levy credit event is stored")]
        public void ThenTheDataForEachLevyCreditEventIsStored()
        {

            _records = _azureTableService.GetRecords(EmployerAccountId.ToString())
                .ToList();

            double expectedRecordsoBeSaved = 2;
            Assert.AreEqual(expectedRecordsoBeSaved, _records.Count(), message: $"Only {expectedRecordsoBeSaved} record should validate and be saved to the database");
        }
        
        [Then(@"all of the data stored is correct")]
        public void ThenAllOfTheDataStoredIsCorrect()
        {
            var second = _records[1];
            Assert.AreEqual(201, second.Amount);
        }

        private IEnumerable<string> Data()
        {
            return
                new List<LevyDeclarationEvent> {
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 201,
                        TransactionDate = DateTime.Now,
                        PayrollDate = DateTime.Now,
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 100,
                        TransactionDate = DateTime.Now.AddMonths(-12),
                        PayrollDate = DateTime.Now.AddMonths(-12),
                        Scheme = "Not sure"
                    },
                    new LevyDeclarationEvent {
                        EmployerAccountId = EmployerAccountId,
                        Amount = 100,
                        TransactionDate = DateTime.Now.AddMonths(-25),
                        PayrollDate = DateTime.Now.AddMonths(-15),
                        Scheme = "Not sure"
                    }
                }
                .Select(m => JsonConvert.SerializeObject(m));
        }
    }
}
