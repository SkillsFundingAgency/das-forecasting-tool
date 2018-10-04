using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    [Binding]
    public class EstimateCostsPageSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public EstimateCostsPageSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        [Given(@"that I'm on the Estimated Costs page")]
        public void GivenThatImOnTheEstimatedCostsPage()
        {
            //EmployerHash = "M6PKPG";
            string EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            string EmployeePassword = "Dell1507";
            //WebSite.SetEmployeeHash(EmployerHash);
            //Console.WriteLine($"Employer hash: {EmployerHash}");

            if (!IsLocalhost)
            {
                var loginPage = NavigateToLoginPage(_driver);
                loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);
            }

            if (!IsLocalhost)
            {
                var accountHomepage = NavigateToAccountHomePage(_driver);
                var financePage = accountHomepage.OpenFinance();
                financePage.OpenFundingProjection();
            }

            var startPage = NavigateToEstimateFundsStartPage(_driver);

            var addApprenticeshipPage = startPage.ClickStartForAccountWithApprenticeships();
        }

        [When(@"I click on the '(.*)' link")]
        public void WhenIClickOnTheLink(string p0)
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            if (p0 == "Add more apprenticeships...")
            {
                var addPage = estimateCostsPage.AddApprenticeships();
            }
            if (p0 == "Apprenticeships added")
            {
                estimateCostsPage.SwitchToApprenticeshipsAddedTab();
            }
        }

        [Then(@"I am taken to the Add Apprenticeships page")]
        public void ThenIAmTakenToTheAddApprenticeshipsPage()
        {
            AddApprenticeshipsToEstimateCostPage page = new AddApprenticeshipsToEstimateCostPage(_driver);
            Assert.IsTrue(page.PageHeader.Displayed);
        }

        [Then(@"I am taken to the Apprenticeships added tab")]
        public void ThenIAmTakenToTheApprenticeshipsAddedTab()
        {
            EstimateCostsPage page = new EstimateCostsPage(_driver);
            Assert.IsTrue(page.IsApprenticeshipsAddedTabActive);
        }

        [When(@"I view the remaining transfer allowance tab")]
        public void WhenIViewTheRemainingTransferAllowanceTab()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            if (!_driver.Url.Contains("apprenticeship/add"))
            {
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
                {
                    estimateCostsPage.RemoveFirstApprenticeship();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }

                ClickOnElement(_driver, "a[href*='apprenticeship/add']");
            }

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.UseTransferAllowance.Click();
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2018");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
            estimateCostsPage.SwitchToRemainingTransferAllowanceTab();
        }

        [Then(@"the first month in the table is the earliest month in which a modelled apprenticeship payment is made \(the month after the earliest start date\)")]
        public void ThenTheFirstMonthInTheTableIsTheEarliestMonthInWhichAModelledApprenticeshipPaymentIsMadeTheMonthAfterTheEarliestStartDate()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var remainingTransferAllowanceTable = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            foreach (var row in remainingTransferAllowanceTable)
            {
                Assert.IsTrue(DateTime.Parse(remainingTransferAllowanceTable[0].Date) <= DateTime.Parse(row.Date));
            }
        }

        [Then(@"the first month's transfer allowance value is correct")]
        public void ThenTheFirstMonthsTransferAllowanceValueIsCorrect()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var remainingTransferAllowanceTable = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            Assert.AreEqual(remainingTransferAllowanceTable[0].RemainingTransferAllowance, "-£400");
        }

        [Then(@"the first month's modelled costs is correct")]
        public void ThenTheFirstMonthsModelledCostsIsCorrect()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var remainingTransferAllowanceTable = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            Assert.AreEqual(remainingTransferAllowanceTable[0].YourCurrentTransferCosts, "£0");
        }

        [Then(@"each subsequent month's transfer allowance is correct")]
        public void ThenEachSubsequentMonthsTransferAllowanceIsCorrect()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var remainingTransferAllowanceTable = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].RemainingTransferAllowance, "-£400");
                        break;
                    case 1:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].RemainingTransferAllowance, "-£800");
                        break;
                    case 2:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].RemainingTransferAllowance, "-£1,200");
                        break;
                    case 3:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].RemainingTransferAllowance, "-£1,600");
                        break;
                    case 4:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].RemainingTransferAllowance, "-£2,000");
                        break;
                }
            }
        }

        [Then(@"each subsequent month's modelled costs is correct")]
        public void ThenEachSubsequentMonthsModelledCostsIsCorrect()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var remainingTransferAllowanceTable = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].YourCurrentTransferCosts, "£0");
                        break;
                    case 1:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].YourCurrentTransferCosts, "£0");
                        break;
                    case 2:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].YourCurrentTransferCosts, "£0");
                        break;
                    case 3:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].YourCurrentTransferCosts, "£0");
                        break;
                    case 4:
                        Assert.AreEqual(remainingTransferAllowanceTable[i].YourCurrentTransferCosts, "£0");
                        break;
                }
            }
        }

        [Then(@"in the each April the transfer allowance value resets to its original value")]
        public void ThenInTheEachAprilTheTransferAllowanceValueResetsToItsOriginalValue()
        {
            //check what this means
        }

        [Then(@"the last month is the month in which the last completion payment for a modelled apprenticeship will be made")]
        public void ThenTheLastMonthIsTheMonthInWhichTheLastCompletionPaymentForAModelledApprenticeshipWillBeMade()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var remainingTransferAllowanceTable = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            Assert.AreEqual(remainingTransferAllowanceTable.Last().Date, "Mar 2019");

        }

        [When(@"I click on '(.*)'")]
        public void WhenIClickOn(string p0)
        {
            if (p0 == "What does the table show")
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
                if (!_driver.Url.Contains("apprenticeship/add"))
                {
                    var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                    while (isAnyapprenticeshipExist)
                    {
                        estimateCostsPage.RemoveFirstApprenticeship();
                        isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                    }

                    ClickOnElement(_driver, "a[href*='apprenticeship/add']");
                }

                AddApprenticeshipsToEstimateCostPage addApprenticeshipPage = new AddApprenticeshipsToEstimateCostPage(_driver);
                addApprenticeshipPage.UseTransferAllowance.Click();
                addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
                addApprenticeshipPage.PageHeader.ClickThisElement();
                addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
                addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
                addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
                addApprenticeshipPage.ContinueButton.ClickThisElement();
                estimateCostsPage.SwitchToRemainingTransferAllowanceTab();
                estimateCostsPage.ClickOnWhatDoesThisTableShowLink();
            }
        }

        [Then(@"the relevant text is displayed")]
        public void ThenTheRelevantTextIsDisplayed()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            Assert.IsTrue(estimateCostsPage.IsTextWhatDoesThisTableShowDisplayed);
        }

        [Then(@"the text matches the design")]
        public void ThenTheTextMatchesTheDesign()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            IList<string> textFromPage = estimateCostsPage.WhatDoesThisTableShowText;
            IList<string> expectedText = new List<string>();
            expectedText.Add("The ‘Remaining transfer allowance’ table shows a projection of your remaining transfer allowance and includes any actual training costs already committed for transfers. Your transfer allowance is re-calculated every tax year and will be available to use from May of each year.");
            expectedText.Add("The ‘Your current transfer costs’ column shows the cost of the apprenticeships that you’ve already agreed to fund with your transfer funds.");
            Assert.AreEqual(textFromPage, expectedText);
        }

        [When(@"I have modelled apprenticeships")]
        public void WhenIHaveModelledApprenticeships()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            if (!_driver.Url.Contains("apprenticeship/add"))
            {
                var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                while (isAnyapprenticeshipExist)
                {
                    estimateCostsPage.RemoveFirstApprenticeship();
                    isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
                }

                ClickOnElement(_driver, "a[href*='apprenticeship/add']");
                //driver.FindElement(By.CssSelector("a[href*='apprenticeship/add']")).Click();
            }

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage = new AddApprenticeshipsToEstimateCostPage(_driver);
            addApprenticeshipPage.UseTransferAllowance.Click();
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
        }

        [When(@"I can afford those apprenticeships from my transfer allowance")]
        public void WhenICanAffordThoseApprenticeshipsFromMyTransferAllowance()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            estimateCostsPage.SwitchToRemainingTransferAllowanceTab();
            //Data needs to be inserted into db
            ScenarioContext.Current.Pending();
        }

        [Then(@"the banner message says that I can fund the apprenticeships")]
        public void ThenTheBannerMessageSaysThatICanFundTheApprenticeships()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"there is no highlighting of any rows in the remaining transfer allowance table")]
        public void ThenThereIsNoHighlightingOfAnyRowsInTheRemainingTransferAllowanceTable()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I cannot afford those apprenticeships from my transfer allowance")]
        public void WhenICannotAffordThoseApprenticeshipsFromMyTransferAllowance()
        {
            //add a code to check that apprenticeships cannot be afforded from transfer allowance
        }

        [Then(@"the banner message says that I can't fund the apprenticeships")]
        public void ThenTheBannerMessageSaysThatICantFundTheApprenticeships()
        {
            //there is no banner currently
        }

        [Then(@"there is highlighting of any rows in the remaining transfer allowance table")]
        public void ThenThereIsHighlightingOfAnyRowsInTheRemainingTransferAllowanceTable()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            estimateCostsPage.SwitchToRemainingTransferAllowanceTab();
            Assert.IsTrue(estimateCostsPage.VerifyThatThereIsHighlightingOfAnyRow);
        }

        [When(@"I have removed my last modelled apprenticeships")]
        public void WhenIHaveRemovedMyLastModelledApprenticeships()
        {
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
        }

        [Then(@"the table is not displayed")]
        public void ThenTheTableIsNotDisplayed()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            Assert.IsTrue(estimateCostsPage.IsRemainingTransferAllowanceTableDisplayed);
        }

        [Then(@"the message '(.*)' is displayed in their place")]
        public void ThenTheMessageIsDisplayedInTheirPlace(string p0)
        {
            if (p0 == "You have not selected any apprenticeships...")
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
                Assert.IsTrue(estimateCostsPage.IsYouHaveNotSelectedAnyApprenticeshipsMessageDisplayed);
            }
        }

        [Then(@"the '(.*)' text is not displayed")]
        public void ThenTheTextIsNotDisplayed(string p0)
        {
            if (p0 == "What does this table show")
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
                Assert.IsTrue(estimateCostsPage.IsTextWhatDoesThisTableShowDisplayed);
            }
        }
    }
}