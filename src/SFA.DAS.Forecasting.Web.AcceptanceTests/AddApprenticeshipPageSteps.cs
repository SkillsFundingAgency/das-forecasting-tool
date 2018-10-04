using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class AddApprenticeshipPageSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public AddApprenticeshipPageSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        [Given(@"that I'm on the Add Apprenticeship page")]
        public void GivenThatIMOnTheAddApprenticeshipPage()
        {
            string EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            string EmployeePassword = "Dell1507";

            var loginPage = NavigateToLoginPage(_driver);
            loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);

            var accountHomepage = NavigateToAccountHomePage(_driver);
            var financePage = accountHomepage.OpenFinance();
            financePage.OpenFundingProjection();

            NavigateToEstimateFundsStartPage(_driver);

            EstimateFundsStartPage page =
                new EstimateFundsStartPage(_driver);
            page.ClickStartForAccountWithoutApprenticeships();
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            if (!_driver.Url.Contains("apprenticeship/add"))
            {
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                {
                    estimateCostsPage.RemoveFirstApprenticeship();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }
            }

            if (!_driver.Url.Contains("apprenticeship/add"))
            {
                EstimateCostsPage fundsStartPage = new EstimateCostsPage(_driver);
                fundsStartPage.AddApprenticeshipsButton.Click();
            }
        }

        [When(@"I select '(.*)'")]
        public void WhenISelect(string p0)
        {
            if (p0 == "Check if I can fund these")
            {
                AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                    new AddApprenticeshipsToEstimateCostPage(_driver);
                addApprenticeshipPage.ContinueButton.ClickThisElement();
            }
        }

        [When(@"I do not select an apprenticeship")]
        public void WhenIDoNotSelectAnApprenticeship()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("£27,000");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
        }

        [Then(@"the '(.*)' error is displayed in line and at the top of the page")]
        public void ThenTheErrorIsDisplayedInLineAndAtTheTopOfThePage(string p0)
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);

            if (p0 == "You must choose 1 apprenticeship")
            {
                Assert.IsTrue(addApprenticeshipPage.IsErrorBoxDisplayed);
                Assert.IsTrue(addApprenticeshipPage.IsYouMustChooseOneApprenticeshipErrorDisplayed);
            }

            if (p0 == "Make sure you have at least 1 or more apprentices")
            {
                Assert.IsTrue(addApprenticeshipPage.IsErrorBoxDisplayed);
                Assert.IsTrue(addApprenticeshipPage.IsMakeSureYouHaveAtLeastOneOrMoreApprenticesErrorDisplayed);
            }

            if (p0 == "The start month was not entered")
            {
                Assert.IsTrue(addApprenticeshipPage.IsErrorBoxDisplayed);
                Assert.IsTrue(addApprenticeshipPage.IsTheStartMonthWasNotEnteredErrorDisplayed);
            }

            if (p0 == "The start year was not entered")
            {
                Assert.IsTrue(addApprenticeshipPage.IsErrorBoxDisplayed);
                Assert.IsTrue(addApprenticeshipPage.IsTheStartYearWasNotEnteredErrorDisplayed);
            }

            if (p0 == "The total training cost was not entered")
            {
                Assert.IsTrue(addApprenticeshipPage.IsErrorBoxDisplayed);
                Assert.IsTrue(addApprenticeshipPage.IsYouMustEnterAnumberThatIsAboveZeroErrorDisplayed);
            }

            if (p0 == "The start date must be within the next 4 years")
            {
                Assert.IsTrue(addApprenticeshipPage.IsErrorBoxDisplayed);
                Assert.IsTrue(addApprenticeshipPage.IsTheStartDateCannotBeInThePastErrorDisplayed);
            }
        }

        [When(@"I do not enter the number of apprenticeship")]
        public void WhenIDoNotEnterTheNumberOfApprenticeship()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
        }

        [When(@"I enter a start date before the current month")]
        public void WhenIEnterAStartDateBeforeTheCurrentMonth()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            string month = (DateTime.Now.Month - 1).ToString();
            string year = (DateTime.Now.Year - 1).ToString();
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement(month);
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement(year);
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
        }

        [When(@"I do not enter a start date month")]
        public void WhenIDoNotEnterAStartDateMonth()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.Clear();
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
        }

        [When(@"I do not enter a start date year")]
        public void WhenIDoNotEnterAStartDateYear()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.Clear();
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
        }

        [When(@"I do not enter a total cost")]
        public void WhenIDoNotEnterATotalCost()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
        }

        [When(@"i choose a standard apprenticeship")]
        public void WhenIChooseAStandardApprenticeship()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
        }

        [When(@"i choose a Framework apprenticeship")]
        public void WhenIChooseAFrameworkApprenticeship()
        {
            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Accounting: Accounting, Level: 4");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("5,000");
        }

        [Then(@"the apprenticeship is added")]
        public void ThenTheApprenticeshipIsAdded()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            List<EstimateCostsPage.ApprenticeshipsTableRow> apprenticeshipsTable =
                estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.IsNotEmpty(apprenticeshipsTable);
        }
    }
}
