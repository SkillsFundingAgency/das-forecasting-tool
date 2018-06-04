using FluentAssertions;
using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class CalculateCo_InvestmentAmountsSteps : StepsBase
    {
        [Scope(Feature = "Calculate co-investment amounts [CI-570]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Then(@"the balance should be (.*)")]
        public void ThenTheBalanceShouldBe(int p0)
        {
            var projection = AccountProjections
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .Skip(1)
                .First();

            projection.FutureFunds.Should().Be(0);
        }

        [Then(@"the employer co-investment amount is 10% of the negative balance")]
        public void ThenTheEmployerCo_InvestmentAmountIsOfTheNegativeBalance()
        {
            var projection = AccountProjections
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .Skip(1)
                .First();

            projection.CoInvestmentEmployer.Should().Be(100);
        }
        
        [Then(@"the government co-investment amount is 90% of the negative value")]
        public void ThenTheGovernmentCo_InvestmentAmountIsOfTheNegativeValue()
        {
            var projection = AccountProjections
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .Skip(1)
                .First();

            projection.CoInvestmentGovernment.Should().Be(900);
        }
    }
}