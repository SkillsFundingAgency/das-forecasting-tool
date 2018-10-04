using NUnit.Framework;
using OpenQA.Selenium;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    [Binding]
    public class AddApprenticeshipSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public AddApprenticeshipSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        [When(@"I click on the Add link")]
        public void WhenIClickOnTheLink()
        {
            EstimateCostsPage page = new EstimateCostsPage(_driver);
            page.AddApprenticeships();
        }

        [Then(@"I am on the add apprenticeship page")]
        public void ThenIAmOnTheAddApprenticeshipPage()
        {
            AddApprenticeshipsToEstimateCostPage page = new AddApprenticeshipsToEstimateCostPage(_driver);
            Assert.IsTrue(page.PageHeader.Displayed);
        }

        [When(@"I select '(.*)' from drop down")]
        public void WhenISelectFromdropdown(string standardName)
        {
            AddApprenticeshipsToEstimateCostPage page = new AddApprenticeshipsToEstimateCostPage(_driver);
            page.SelectApprenticeshipDropdown.SelectDropDown(_driver, standardName);
        }
    }
}
