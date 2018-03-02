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
        }

        [Then(@"the balance should be (.*)")]
        public void ThenTheBalanceShouldBe(int p0)
        {
            var projection = AccountProjections.First();
            projection.FutureFunds.Should().Be(0);
        }

        [Then(@"the employer co-investment amount is 10% of the negative balance")]
        public void ThenTheEmployerCo_InvestmentAmountIsOfTheNegativeBalance()
        {
            var projection = AccountProjections.First();
            projection.CoInvestmentEmployer.Should().Be(500);
        }
        
        [Then(@"the government co-investment amount is 90% of the negative value")]
        public void ThenTheGovernmentCo_InvestmentAmountIsOfTheNegativeValue()
        {
            var projection = AccountProjections.First();
            projection.CoInvestmentGovernment.Should().Be(4500);
        }
    }
}