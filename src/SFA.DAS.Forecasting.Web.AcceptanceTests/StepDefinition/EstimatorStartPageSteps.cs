using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Threading;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class EstimatorStartPageSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public EstimatorStartPageSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        [Given(@"that I am an employer with predefined projections")]
        public void GivenThatIAmAnEmployerWithPredefinedProjections()
        {
            //obsoloted logic
            /*EmployerHash = "M6PKPG";
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";
            WebSite.SetEmployeeHash(EmployerHash);
            Console.WriteLine($"Employer hash: {EmployerHash}");*/
        }

        [Given(@"that I'm on the estimator start page")]
        public void GivenThatIMOnTheEstimatorStartPage()
        {
            //EmployerHash = "M6PKPG";
            string EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            string EmployeePassword = "Dell1507";
            //WebSite.SetEmployeeHash(EmployerHash);
            Console.WriteLine("Employer hash: M6PKPG");

            if (!IsLocalhost && !DoesPageTextContain(_driver, "Your accounts"))
            {
                LoginPage loginPage = NavigateToLoginPage(_driver);
                loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);
            }

            if (!IsLocalhost)
            {
                AccountHomePage accountHomepage = NavigateToAccountHomePage(_driver);
                var financePage = accountHomepage.OpenFinance();
                financePage.OpenFundingProjection();
            }

            NavigateToEstimateFundsStartPage(_driver);
        }

        [When(@"I have no current modelled apprenticeships")]
        public void WhenIHaveNoCurrentModelledApprenticeships()
        {
            EstimateFundsStartPage page =
                new EstimateFundsStartPage(_driver);
            page.ClickStartForAccountWithoutApprenticeships();
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            if (!CurrentUrl(_driver).Contains("apprenticeship/add"))
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
            if (CurrentUrl(_driver).Contains("forecasting/estimations/start"))
            {
                EstimateFundsStartPage fundsStartPage1 = new EstimateFundsStartPage(_driver);
                fundsStartPage1.ClickStartForAccountWithApprenticeships();
            }
            if (!CurrentUrl(_driver).Contains("apprenticeship/add"))
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
                {
                    estimateCostsPage.RemoveFirstApprenticeship();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }
            }
            if (!CurrentUrl(_driver).Contains("apprenticeship/add"))
            {
                EstimateFundsStartPage fundsStartPage = new EstimateFundsStartPage(_driver);
                fundsStartPage.ClickStartForAccountWithApprenticeships();
            }
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.UseTransferAllowance.Click();
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
        }

        [Then(@"by clicking the Start button I am taken to the Add apprenticeship page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheAddApprenticeshipPage()
        {
            if (!CurrentUrl(_driver).Contains("forecasting/estimations/default/apprenticeship/add"))
            {
                EstimateCostsPage page =
                    new EstimateCostsPage(_driver);
                page.AddApprenticeshipsButton.Click();
            }
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            Assert.IsTrue(addApprenticeshipPage.IsPageVisible());
        }

        [Then(@"by clicking the Start button I am taken to the Estimated costs page")]
        public void ThenByClickingTheStartButtonIAmTakenToTheEstimatedCostsPage()
        {
            NavigateToEstimateFundsStartPage(_driver);
        }

        [Then(@"the Remaining transfers allowance tab is displayed")]
        public void ThenTheRemainingTransfersAllowanceTabIsDisplayed()
        {
            EstimateCostsPage page = new EstimateCostsPage(_driver);
            Assert.IsTrue(page.RemainingTransferAllowanceTabButton.Displayed);
        }

        [Then(@"the previously modelled apprenticeships costs are displayed")]
        public void ThenThePreviouslyModelledApprenticeshipsCostsAreDisplayed()
        {
            EstimateCostsPage page = new EstimateCostsPage(_driver);
            Assert.IsTrue(page.TotalCostLabel.Displayed);
            Assert.AreEqual("£18,000", page.TotalCostLabel.Text);
        }
    }
}
