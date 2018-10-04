using NUnit.Framework;
using OpenQA.Selenium;
using SFA.DAS.Forecasting.Web.Automation;
using SFA.DAS.Forecasting.Web.Automation.Estimation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    public class EditApprenticehipSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public EditApprenticehipSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }


        [When(@"I navigate to the Estimated costs page")]
        public void WhenINavigateToTheEstimatedCostsPage()
        {
            NavigateToEstimageCostsPage(_driver);
        }

        [When(@"I click on the Edit link")]
        public void WhenIClickOnTheLink()
        {
            EstimateCostsPage page = new EstimateCostsPage(_driver);
            var editPage = page.EditApprenticeships();
        }

        [Then(@"I am on the edit apprenticeship page")]
        public void ThenIAmOnTheEditApprenticeshipPage()
        {
            EditApprenticeshipsPage page = new EditApprenticeshipsPage(_driver);
            Assert.AreEqual("Edit apprenticeships in your current estimate", page.Heading.Text);
        }
    }
}
