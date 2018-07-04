using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Security
{
    [Binding]
    public class RedirectToLoginPageSteps
    {
        [Given(@"The user is not logged in")]
        public void GivenTheUserIsNotLoggedIn()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"The user is logged in")]
        public void GivenTheUserIsLoggedIn()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
