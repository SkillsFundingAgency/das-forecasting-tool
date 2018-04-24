using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class EstimatorStartPageSteps: StepsBase
    {
        [Given(@"that I am an employer with predefined projections")]
        public void GivenThatIAmAnEmployerWithPredefinedProjections()
        {
            EmployerHash = "M6PKPG";
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";
            WebSite.SetEmployeeHash(EmployerHash);
            Console.WriteLine($"Employer hash: {EmployerHash}");
        }

        [Given(@"that I'm on the estimator start page")]
        public void GivenThatIMOnTheEstimatorStartPage()
        {
            var accountHomepage = WebSite.NavigateToAccountHomePage();
            var financePage = accountHomepage.OpenFinance();
            financePage.OpenFundingProjection();
            WebSite.NavigateToEstimateFundsStartPage();
        }
        
        [When(@"I have no current modelled apprenticeships")]
        public void WhenIHaveNoCurrentModelledApprenticeships()
        {
            var page = WebSite.NavigateToEstimageCostsPage();
            var isAnyapprenticeshipExist = page.IsApprenticeshipsTableVisible();
            while (isAnyapprenticeshipExist)
            {
                page.RemoveFirstApprenticeship();
                isAnyapprenticeshipExist = page.IsApprenticeshipsTableVisible();
            }
            WebSite.NavigateToEstimateFundsStartPage();
        }
        
        [When(@"I have current modelled apprenticeships")]
        public void WhenIHaveCurrentModelledApprenticeships()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"by clicking the Start button I am taken to the Add apprenticeship page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheAddApprenticeshipPage()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"by clicking the Start button I am taken to the Estimated costs page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheEstimatedCostsPage()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the Remaining transfers allowance tab is displayed")]
        public void ThenTheRemainingTransfersAllowanceTabIsDisplayed()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the previously modelled apprenticeships costs are displayed")]
        public void ThenThePreviouslyModelledApprenticeshipsCostsAreDisplayed()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
