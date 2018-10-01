using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Threading;
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

<<<<<<< HEAD
            if (!WebSite.IsLocalhost && !WebSite.DoesPageTextContain("Your accounts"))
=======
<<<<<<< HEAD
            if (!WebSite.IsLocalhost && !WebSite.DoesPageTextContain("Your accounts"))
=======
            if (!WebSite.IsLocalhost)
>>>>>>> e53db1e22edd86dc2964aebe726d6566c7e021a5
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
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
            EstimateFundsStartPage page =
                new EstimateFundsStartPage(WebSite.getDriver());
            page.ClickStartForAccountWithoutApprenticeships();
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            if (!WebSite.CurrentUrl.Contains("apprenticeship/add"))
            {
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
                {
                    estimateCostsPage.RemoveFirstApprenticeship();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }
            }
        }
        
        [When(@"I have current modelled apprenticeships")]
        public void WhenIHaveCurrentModelledApprenticeships()
        {
            if (WebSite.CurrentUrl.Contains("forecasting/estimations/start"))
            {
                EstimateFundsStartPage fundsStartPage1 = new EstimateFundsStartPage(WebSite.getDriver());
                fundsStartPage1.ClickStartForAccountWithApprenticeships();
            }
<<<<<<< HEAD
            if (!WebSite.CurrentUrl.Contains("apprenticeship/add"))
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
                {
                    estimateCostsPage.RemoveFirstApprenticeship();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }
            }
            if (!WebSite.CurrentUrl.Contains("apprenticeship/add"))
            {
                EstimateFundsStartPage fundsStartPage = new EstimateFundsStartPage(WebSite.getDriver());
                fundsStartPage.ClickStartForAccountWithApprenticeships();
            }
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(WebSite.getDriver());
=======
            var startPage = WebSite.NavigateToEstimateFundsStartPage();
            var addApprenticeshipPage = startPage.ClickStartForAccountWithoutApprenticeships();
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
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
            if (!WebSite.CurrentUrl.Contains("forecasting/estimations/default/apprenticeship/add"))
            {
                EstimateCostsPage page =
                    new EstimateCostsPage(WebSite.getDriver());
                page.AddApprenticeshipsButton.Click();
            }
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(WebSite.getDriver());
            Assert.IsTrue(addApprenticeshipPage.IsPageVisible());
        }
        
        [Then(@"by clicking the Start button I am taken to the Estimated costs page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheEstimatedCostsPage()
        {
            WebSite.NavigateToEstimateFundsStartPage();
            EstimateCostsPage estimatedCostsPage =
                new EstimateCostsPage(WebSite.getDriver());
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
