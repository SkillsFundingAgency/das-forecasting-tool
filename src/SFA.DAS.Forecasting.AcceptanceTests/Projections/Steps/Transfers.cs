using FluentAssertions;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class Transfers : StepsBase
    {
        [Scope(Feature = "Calculate monthly cost of actual transfer commitments (Sending Employer - CI-619)")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Then(@"should have following projections")]
        public void ThenShouldHaveFollowingProjections(Table table)
        {
            var s = table.CreateSet<TestAccountProjection>().ToList();

            foreach (var item in s)
            {
                var projections = AccountProjections.Single(m => m.Month == item.Date.Month && m.Year == item.Date.Year);
                projections.TransferOutTotalCostOfTraining.Should().Be(item.TransferOutTotalCostOfTraining);
            }
        }

    }
}
