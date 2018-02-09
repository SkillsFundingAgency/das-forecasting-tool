using FluentAssertions;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub;
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
    public class PreLoadLevyEventsSteps : StepsBase
    {
        private const string FeatureName= "PreLoadLevyEvents";
        private static readonly IReadOnlyList<string> TableStorageIds = new List<string> { "497_2018_19_1", "498_2018_19_1", "499_2018_19_1" };
        private static string Url = Path.Combine(Config.FunctionBaseUrl, "LevyDeclarationPreLoadHttpFunction");

        private static Host _h;
        private AzureTableService _azureTableService;

        [Scope(Feature = FeatureName)]
        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            _h = new Host();
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
            Thread.Sleep(1000);
        }


        [Scope(Feature = FeatureName)]
        [BeforeScenario]
        public void BeforeScenario()
        {
            _azureTableService = new AzureTableService(Config.AzureStorageConnectionString, Config.LevyDeclarationsTable);
            _azureTableService.EnsureExists();

            foreach (var id in TableStorageIds)
            {
                _azureTableService.DeleteEntities(id);
            }
        }

        [Scope(Feature = FeatureName)]
        [AfterScenario]
        public void AfterSecnario()
        {
            foreach (var id in TableStorageIds)
            {
                _azureTableService.DeleteEntities(id);
            }

            Thread.Sleep(1000);
        }

        [Given(@"I trigger function for 3 employers to have their data loaded.")]
        public async Task ITriggerFunction()
        {
            var item = "{\"EmployerAccountIds\":[\"MJK9XV\",\"MGXLRV\",\"MPN4YM\"],\"PeriodYear\":\"2018-19\",\"PeriodMonth\":1}";

            var client = new HttpClient();
            await client.PostAsync(Url, new StringContent(item));
        }

        [When(@"data have been processed")]
        public void DataHaveBeenProcessed()
        {
            Thread.Sleep(1000);
        }

        [Then(@"there will be (.*) records in the storage")]
        public void ThereWillBeThreeRecordsInTheStorage(int expectedCount)
        {
            var declarations = TableStorageIds.Select(m => _azureTableService.GetRecords<LevyDeclarationUpdatedMessage>(m).SingleOrDefault());

            declarations.Count().Should().Be(expectedCount);
        }
        // all records should have the latest data
        [Then(@"all records should have the latest data")]
        public void AllRecordsShouldHaveTheLatestData()
        {
            var declarations = TableStorageIds.Select(m => _azureTableService.GetRecords<LevyDeclarationUpdatedMessage>(m).SingleOrDefault());

            foreach (var declaration in declarations)
            {
                declaration.LevyDeclaredInMonth.Should().Be(200);
                declaration.PayrollYear.Should().Be("2018-19");
                declaration.PayrollMonth.Should().Be(1);
                declaration.CreatedAt.Should().NotBeCloseTo(DateTime.UtcNow);
            }
        }
    }
}
