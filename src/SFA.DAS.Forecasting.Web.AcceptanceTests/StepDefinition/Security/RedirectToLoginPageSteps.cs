using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Security
{
    [Binding]
    public class RedirectToLoginPageSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public RedirectToLoginPageSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

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
