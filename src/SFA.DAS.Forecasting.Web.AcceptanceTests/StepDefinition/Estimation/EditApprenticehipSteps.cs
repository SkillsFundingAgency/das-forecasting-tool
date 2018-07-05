using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using SFA.DAS.Forecasting.Web.Automation.Estimation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    public class EditApprenticehipSteps : StepsBase
    {
        [When(@"I navigate to the Estimated costs page")]
        public void WhenINavigateToTheEstimatedCostsPage()
        {
            var costsPage = WebSite.NavigateToEstimageCostsPage();
            Set(costsPage);
        }

        [When(@"I click on the Edit link")]
        public void WhenIClickOnTheLink()
        {
            var page = Get<EstimateCostsPage>();
            var editPage = page.EditApprenticeships();
            Set(editPage);
        }

        [Then(@"I am on the edit apprenticeship page")]
        public void ThenIAmOnTheEditApprenticeshipPage()
        {
            var page = Get<EditApprenticeshipsPage>();
            Assert.AreEqual("Edit apprenticeships in your current estimate", page.Heading.Text);
            Set(page as AddEditApprenticeshipPage);
        }

    }
}
