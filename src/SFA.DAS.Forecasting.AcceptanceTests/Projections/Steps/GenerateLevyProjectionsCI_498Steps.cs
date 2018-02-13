using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class GenerateLevyProjectionsCI_498Steps: StepsBase
    {
        [Scope(Feature = "Generate Levy Projections [CI-498]")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
        }

        [Given(@"The following levy declaration has been recorded")]
        public void GivenTheFollowingLevyDeclarationHasBeenRecorded(Table table)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"the account projection is generated")]
        public void WhenTheAccountProjectionIsGenerated()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"calculated levy credit value should be the amount declared for the single linked PAYE scheme")]
        public void ThenCalculatedLevyCreditValueShouldBeTheAmountDeclaredForTheSingleLinkedPAYEScheme()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"each future month's forecast levy credit is the same")]
        public void ThenEachFutureMonthSForecastLevyCreditIsTheSame()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
