using FluentAssertions;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub;
using SFA.DAS.Forecasting.AcceptanceTests.Services;
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
        private static string Url = Path.Combine(Config.LevyFunctionUrl, "LevyDeclarationPreLoadHttpFunction");

        private static ApiHost _apiHost;
        private AzureTableService _azureTableService;

        [Scope(Feature = FeatureName)]
        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            _apiHost = new ApiHost();
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
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
            // ToDo: Query SQL
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
