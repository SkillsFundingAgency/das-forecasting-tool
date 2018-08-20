using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    public class AddApprenticeshipSteps : StepsBase
    {
        [When(@"I click on the Add link")]
        public void WhenIClickOnTheLink()
        {
            var page = Get<EstimateCostsPage>();
            var addPage = page.AddApprenticeships();
            Set(addPage);
        }

        [Then(@"I am on the add apprenticeship page")]
        public void ThenIAmOnTheAddApprenticeshipPage()
        {
            var page = Get<AddApprenticeshipsToEstimateCostPage>();
            Assert.IsTrue(page.PageHeader.Displayed);
            Set(page as AddEditApprenticeshipPage);
        }


        [When(@"I select '(.*)' from drop down")]
        public void WhenISelectFromdropdown(string standardName)
        {
            var page = Get<AddEditApprenticeshipPage>() as AddApprenticeshipsToEstimateCostPage;
            page.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), standardName);
            Set(page as AddEditApprenticeshipPage);
            
        }
    }
}
