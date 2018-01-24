using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy
{
    [TestFixture]
    public class AddingLevyDeclarations
    {
        // ToDo: Move to config...
        private const string TableName = "LevyDeclarations";
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private const long EmployerAccountId = 1111;

        private AzureTableService _azureTableService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _azureTableService = new AzureTableService(ConnectionString, TableName);
            _azureTableService.EnsureExcists();

            // Start Functions
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _azureTableService.DeleteEntities(EmployerAccountId.ToString());
        }

        [Test]
        public async Task Run()
        {
            var client = new HttpClient();

            foreach(var item in Data())
            {
                await client.PostAsync("http://localhost:7071/api/LevyDeclarationEventHttpFunction", new StringContent(item));
                Thread.Sleep(100);
            }

            Thread.Sleep(1000); // retry..

            var records = _azureTableService.GetRecords(EmployerAccountId.ToString())
                .ToList();

            double expectedRecordsoBeSaved = 2;
            Assert.AreEqual(expectedRecordsoBeSaved, records.Count(), message: $"Only {expectedRecordsoBeSaved} record should validate and be saved to the database");

            var second = records[1];
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
