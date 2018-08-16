using System;
using FluentAssertions;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
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
                .First();

            projection.FutureFunds.Should().Be(p0);
        }
        [Then(@"the co-investment amount is zero")]
        public void ThenTheCo_InvestmentAmountIsZero()
        {
            var projection = AccountProjections
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .First();

            Assert.AreEqual(0, projection.CoInvestmentEmployer + projection.CoInvestmentGovernment);
        }

        [Then(@"the employer co-investment amount is 10% of the remaining cost of training")]
        public void ThenTheEmployerCo_InvestmentAmountIsOfTheNegativeBalance()
        {
            var projection = AccountProjections
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .First();

            var currentBalance = Convert.ToDecimal(ScenarioContext.Current["current_balance"]);

            if (currentBalance < 0)
            {
                currentBalance = 0;
            }

            var coInvestmentAmount = projection.CoInvestmentEmployer;
            var costOfTraining = projection.LevyFundedCostOfTraining - currentBalance;

            Assert.AreEqual(0.1m, coInvestmentAmount/costOfTraining);
        }
        
        [Then(@"the government co-investment amount is 90% of the remaining cost of training")]
        public void ThenTheGovernmentCo_InvestmentAmountIsOfTheNegativeValue()
        {
            var projection = AccountProjections
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .First();
            var currentBalance = Convert.ToDecimal(ScenarioContext.Current["current_balance"]);

            if (currentBalance < 0)
            {
                currentBalance = 0;
            }

            var coInvestmentAmount = projection.CoInvestmentGovernment;
            var costOfTraining = projection.LevyFundedCostOfTraining - currentBalance;

            Assert.AreEqual(0.9m, coInvestmentAmount/ costOfTraining );
        }
    }
}