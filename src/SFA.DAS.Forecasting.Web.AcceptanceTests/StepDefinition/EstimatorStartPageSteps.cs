using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
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
            EmployerHash = "M6PKPG";
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";
            WebSite.SetEmployeeHash(EmployerHash);
            Console.WriteLine("Employer hash: M6PKPG");

            if (!WebSite.IsLocalhost && !WebSite.DoesPageTextContain("Your accounts"))
            {
                var loginPage = WebSite.NavigateToLoginPage();
                loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);
            }

            if (!WebSite.IsLocalhost)
            {
                var accountHomepage = WebSite.NavigateToAccountHomePage();
                var financePage = accountHomepage.OpenFinance();
                financePage.OpenFundingProjection();
            }

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
        }
        
        [When(@"I have current modelled apprenticeships")]
        public void WhenIHaveCurrentModelledApprenticeships()
        {
            var estimateCostsPage = WebSite.NavigateToEstimageCostsPage();
            var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
            while (isAnyapprenticeshipExist)
            {
                estimateCostsPage.RemoveFirstApprenticeship();
                isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
            }
            var startPage = WebSite.NavigateToEstimateFundsStartPage();
            var addApprenticeshipPage = startPage.ClickStartForAccountWithoutApprenticeships();
            addApprenticeshipPage.UseTransferAllowance.Click();
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
        }
        
        [Then(@"by clicking the Start button I am taken to the Add apprenticeship page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheAddApprenticeshipPage()
        {
            var page = WebSite.NavigateToEstimateFundsStartPage();
            var addApprenticeshipPage = page.ClickStartForAccountWithoutApprenticeships();
            Assert.IsTrue(addApprenticeshipPage.IsPageVisible());
        }
        
        [Then(@"by clicking the Start button I am taken to the Estimated costs page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheEstimatedCostsPage()
        {
            var page = WebSite.NavigateToEstimateFundsStartPage();
            var estimatedCostsPage = page.ClickStartForAccountWithApprenticeships();
            Set(estimatedCostsPage);
        }
        
        [Then(@"the Remaining transfers allowance tab is displayed")]
        public void ThenTheRemainingTransfersAllowanceTabIsDisplayed()
        {
            var page = Get<EstimateCostsPage>();
            Assert.IsTrue(page.RemainingTransferAllowanceTabButton.Displayed);
        }
        
        [Then(@"the previously modelled apprenticeships costs are displayed")]
        public void ThenThePreviouslyModelledApprenticeshipsCostsAreDisplayed()
        {
            var page = Get<EstimateCostsPage>();
            Assert.IsTrue(page.TotalCostLabel.Displayed);
            Assert.AreEqual("£18,000", page.TotalCostLabel.Text);
        }
    }
}
