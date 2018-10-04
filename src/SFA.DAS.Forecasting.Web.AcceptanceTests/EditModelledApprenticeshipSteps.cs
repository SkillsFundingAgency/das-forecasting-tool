﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition;
using SFA.DAS.Forecasting.Web.Automation;
using SFA.DAS.Forecasting.Web.Automation.Estimation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class EditModelledApprenticeshipSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;
        
        public EditModelledApprenticeshipSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.Init("single", "bs");
        }

        [Given(@"that I'm on the Estimated Costs - Apprenticeships Added page")]
        public void GivenThatIMOnTheEstimatedCosts_ApprenticeshipsAddedPage()
        {
            string EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            string EmployeePassword = "Dell1507";

            var loginPage = NavigateToLoginPage(_driver);

            loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);

            var accountHomepage = NavigateToAccountHomePage(_driver);

            var financePage = accountHomepage.OpenFinance();
            financePage.OpenFundingProjection();

            EstimateFundsStartPage page = NavigateToEstimateFundsStartPage(_driver);

            page.ClickStartForAccountWithoutApprenticeships();
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            if (!_driver.Url.Contains("apprenticeship/add"))
            {
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
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

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
        }
        
        [Given(@"that I'm on the Edit apprenticeships page")]
        public void GivenThatIMOnTheEditApprenticeshipsPage()
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
                while (isAnyapprenticeshipExist)
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

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.Click();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.TotalCostInput.Clear();
            addApprenticeshipPage.TotalCostInput.EnterTextInThisElement("18,000");
            addApprenticeshipPage.ContinueButton.ClickThisElement();

            EstimateCostsPage secondEstimateCostsPage = new EstimateCostsPage(_driver);
            secondEstimateCostsPage.EditApprenticeships();
        }

        [When(@"I select edit")]
        public void WhenISelectEdit()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            estimateCostsPage.EditApprenticeships();
        }

        [Then(@"the Edit Apprenticeship page is displayed")]
        public void ThenTheEditApprenticeshipPageIsDisplayed()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            Assert.IsTrue(editApprenticeshipsPage.IsPageLoaded);
        }

        [Then(@"the default details are those of the apprenticeship I wish to edit")]
        public void ThenTheDefaultDetailsAreThoseOfTheApprenticeshipIWishToEdit()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            Assert.IsTrue(editApprenticeshipsPage.ApprentishipName.Equals("Actuary - Level: Actuary"));
            Assert.IsTrue(editApprenticeshipsPage.NumberOfApprenticeships.Equals("1"));
            Assert.IsTrue(editApprenticeshipsPage.ApprenticeshipLength.Equals("36"));
            Assert.IsTrue(editApprenticeshipsPage.StartDateMonth.Equals("10"));
            Assert.IsTrue(editApprenticeshipsPage.StartDateYear.Equals("2019"));
            Assert.IsTrue(editApprenticeshipsPage.TotalFundingCost.Equals("18,000"));
        }

        [When(@"I edit a start date before the current month")]
        public void WhenIEnterAStartDateBeforeTheCurrentMonth()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            string month = (DateTime.Now.Month - 1).ToString();
            string year = (DateTime.Now.Year - 1).ToString();
            editApprenticeshipsPage.ChangeStartDateMonth(month);
            editApprenticeshipsPage.ChangeStartDateYear(year);
            editApprenticeshipsPage.ContinueButton.Click();
        }

        [When(@"I edit a start date before the May Twenty Eighteen")]
        public void WhenIEnterAStartDateBeforeTheMayTwentyEighteen()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            editApprenticeshipsPage.ChangeStartDateMonth("3");
            editApprenticeshipsPage.ChangeStartDateYear("2015");
            editApprenticeshipsPage.ContinueButton.Click();
        }

        [Given(@"the current date is after (.*)")]
        public void GivenTheCurrentDateIsAfter(string p0)
        {
            //just do nothing here
        }
        
        [When(@"I enter a training cost more than the funding cap")]
        public void WhenIEnterATrainingCostMoreThanTheFundingCap()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            editApprenticeshipsPage.ChangeTheFunding("99,000");
            editApprenticeshipsPage.ContinueButton.Click();
        }
        
        [When(@"I update the details \(choose any editable fields\)")]
        public void WhenIUpdateTheDetailsChooseAnyEditableFields()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            editApprenticeshipsPage.ChangeTheNumberOfApprentice("2");
        }
        
        [When(@"I select the '(.*)' button")]
        public void WhenISelectTheButton(string p0)
        {
            if (p0 == "Check if I can fund this")
            {
                EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
                editApprenticeshipsPage.ContinueButton.Click();
            }
        }
        
        [When(@"I change the number of apprentices")]
        public void WhenIChangeTheNumberOfApprentices()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            editApprenticeshipsPage.ChangeTheNumberOfApprentice("2");
            editApprenticeshipsPage.ClickOnPageHeader();
        }

        [When(@"I click on the '(.*)' link on Edit apprenticeships page")]
        public void WhenIClickOnTheLinkOnEditApprenticeshipsPage(string p0)
        {
            //where is that link?
        }

        [When(@"I click on cancel")]
        public void WhenIClickOnCancel()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            editApprenticeshipsPage.ChangeTheNumberOfApprentice("2");
            editApprenticeshipsPage.ClickOnPageHeader();
            editApprenticeshipsPage.ClickOnCancelButton();
        }

        [When(@"I click on the back link")]
        public void WhenIClickOnTheBackLink()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            editApprenticeshipsPage.ChangeTheNumberOfApprentice("2");
            editApprenticeshipsPage.ClickOnPageHeader();
            editApprenticeshipsPage.ClickOnBackLink();
        }

        [Then(@"the error message '(.*)' is displayed")]
        public void ThenTheErrorMessageIsDisplayed(string p0)
        {
            if (p0 == "The start date cannot be in the past")
            {
                EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
                Assert.IsTrue(editApprenticeshipsPage.IsErrorSummaryDisplayed);
                Assert.IsTrue(editApprenticeshipsPage.IsTheStartDateCannotBeInThePastErrorDisplayed);
            }

            if (p0 == "The start date must be on or after 05 2018")
            {
                EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
                Assert.IsTrue(editApprenticeshipsPage.IsErrorSummaryDisplayed);
                Assert.IsTrue(editApprenticeshipsPage.IsTheStartDateCannotBeInThePastErrorDisplayed);
            }

            if (p0 == "The start date cannot be in the past")
            {
                EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
                Assert.IsTrue(editApprenticeshipsPage.IsErrorSummaryDisplayed);
                Assert.IsTrue(editApprenticeshipsPage.IsTheStartDateCannotBeInThePastErrorDisplayed);
            }
        }

        [Then(@"the error message '(.*)' is not displayed")]
        public void ThenTheErrorMessageIsNotDisplayed(string p0)
        {
            if (p0 == "The total cost cant be higher than the government funding cap for this apprenticeship")
            {
               EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
               Assert.IsTrue(estimateCostsPage.IsPageLoaded);
            }
        }

        [Then(@"the '(.*)' page is displayed")]
        public void ThenThePageIsDisplayed(string p0)
        {
            if (p0 == "Estimated cost - Apprenticeships added")
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
                Assert.IsTrue(estimateCostsPage.IsPageLoaded);
            }
        }
        
        [Then(@"the apprenticeship has been updated correctly")]
        public void ThenTheApprenticeshipHasBeenUpdatedCorrectly()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            List<EstimateCostsPage.ApprenticeshipsTableRow> apprenticeshipsTable =
                estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.IsTrue(apprenticeshipsTable[0].NumberOfApprentices.Equals("2"));
        }
        
        [Then(@"the government funding cap is updated")]
        public void ThenTheGovernmentFundingCapIsUpdated()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            Assert.IsTrue(editApprenticeshipsPage.TotalFundingCapDetails.Equals("£36,000"));
        }
        
        [Then(@"the total cost value is updated")]
        public void ThenTheTotalCostValueIsUpdated()
        {
            EditApprenticeshipsPage editApprenticeshipsPage = new EditApprenticeshipsPage(_driver);
            Assert.IsTrue(editApprenticeshipsPage.TotalFundingCost.Equals("36,000"));
        }
        
        [Then(@"I'm taken to https://test-forecasting\.apprenticeships\.sfa\.bis\.gov\.uk/accounts/V(.*)G/forecasting/estimations/Start-Transfer")]
        public void ThenIMTakenToHttpsTest_Forecasting_Apprenticeships_Sfa_Bis_Gov_UkAccountsVGForecastingEstimationsStart_Transfer(string p0)
        {
            //first find the link
        }
        
        [Then(@"the details have not been updated")]
        public void ThenTheDetailsHaveNotBeenUpdated()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            List<EstimateCostsPage.ApprenticeshipsTableRow> apprenticeshipsTable =
                estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.IsFalse(apprenticeshipsTable[0].NumberOfApprentices.Equals("2"));
        }
    }
}
