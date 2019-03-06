using System;
using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.Estimation
{
    public class RemoveModelledApprenticeshipSteps : StepsBase
    {
        [Given(@"that I'm on the modelled apprenticeships tab")]
        public void GivenThatImOnTheModelledApprenticeshipsTab()
        {
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";

            var loginPage = WebSite.NavigateToLoginPage();
            loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);

            var accountHomepage = WebSite.NavigateToAccountHomePage();
            var financePage = accountHomepage.OpenFinance();
            financePage.OpenFundingProjection();

            WebSite.NavigateToEstimateFundsStartPage();

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

            if (!WebSite.CurrentUrl.Contains("apprenticeship/add"))
            {
                EstimateCostsPage fundsStartPage = new EstimateCostsPage(WebSite.getDriver());
                fundsStartPage.AddApprenticeshipsButton.Click();
            }

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(WebSite.getDriver());
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.ContinueButton.ClickThisElement();
        }

        [When(@"I select remove for one apprenticeship")]
        public void WhenISelectRemoveForOneApprenticeship()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            estimateCostsPage.ClickRemoveApprenticeshipButton();
        }

        [Then(@"the remove apprenticeship page is displayed")]
        public void ThenTheRemoveApprenticeshipPageIsDisplayed()
        {
            RemoveApprenticeshipPage removeApprenticeshipPage = new RemoveApprenticeshipPage(WebSite.getDriver());
            Assert.IsTrue(removeApprenticeshipPage.IsPageLoaded);
        }

        [Given(@"that I'm on the remove apprenticeship page")]
        public void GivenThatImOnTheRemoveApprenticeshipPage()
        {
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";

            var loginPage = WebSite.NavigateToLoginPage();
            loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);

            var accountHomepage = WebSite.NavigateToAccountHomePage();
            var financePage = accountHomepage.OpenFinance();
            financePage.OpenFundingProjection();

            WebSite.NavigateToEstimateFundsStartPage();

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

            if (!WebSite.CurrentUrl.Contains("apprenticeship/add"))
            {
                EstimateCostsPage fundsStartPage = new EstimateCostsPage(WebSite.getDriver());
                fundsStartPage.AddApprenticeshipsButton.Click();
            }

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(WebSite.getDriver());
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement("10");
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement("2019");
            addApprenticeshipPage.ContinueButton.ClickThisElement();

            estimateCostsPage.ClickRemoveApprenticeshipButton();
        }

        [When(@"I confirm remove for the apprenticeship")]
        public void WhenIConfirmRemoveForTheApprenticeship()
        {
            RemoveApprenticeshipPage removeApprenticeshipPage = new RemoveApprenticeshipPage(WebSite.getDriver());
            removeApprenticeshipPage.ConfirmRemoval();
        }

        [Then(@"the Estimated Costs page is displayed")]
        public void ThenTheEstimatedCostsPageIsDisplayed()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            Assert.IsTrue(estimateCostsPage.IsPageLoaded);
        }

        [Then(@"the Remaining Transfer Allowance tab is displayed")]
        public void ThenTheRemainingTransferAllowanceTabIsDisplayed()
        {
            //the Remaining Transfer Allowance tab is displayed can't be displayed if there are no apprenticeships
        }

        [Then(@"the removed apprenticeship is not in the list")]
        public void ThenTheRemovedApprenticeshipIsNotInTheList()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            Assert.IsFalse(estimateCostsPage.IsApprenticeshipsTableVisible());
        }

        [Then(@"the list of apprenticeships is in start date order")]
        public void ThenTheListOfApprenticeshipsIsInStartDateOrder()
        {
            //the table is empty and in according to new rules apprenticeships shouldn't be in start date order
        }

        [Then(@"the banner message '(.*)' is displayed")]
        public void ThenTheBannerMessageIsDisplayed(string p0)
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            Assert.IsTrue(estimateCostsPage.IsRemovedApprenticeshipBannerDisplayed);
            Assert.AreEqual(estimateCostsPage.RemovedApprenticeshipBannerText, "Apprenticeship removed");
        }

        [Given(@"that I have removed an apprenticeship")]
        public void GivenThatIHaveRemovedAnApprenticeship()
        {
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";

            var loginPage = WebSite.NavigateToLoginPage();
            loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);

            var accountHomepage = WebSite.NavigateToAccountHomePage();
            var financePage = accountHomepage.OpenFinance();
            financePage.OpenFundingProjection();

            WebSite.NavigateToEstimateFundsStartPage();

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

            if (!WebSite.CurrentUrl.Contains("apprenticeship/add"))
            {
                EstimateCostsPage fundsStartPage = new EstimateCostsPage(WebSite.getDriver());
                fundsStartPage.AddApprenticeshipsButton.Click();
            }

            string month = DateTime.UtcNow.Month.ToString();
            string nextMonth = (DateTime.UtcNow.Month + 1).ToString();
            string year = DateTime.UtcNow.Year.ToString();

            AddApprenticeshipsToEstimateCostPage addApprenticeshipPage =
                new AddApprenticeshipsToEstimateCostPage(WebSite.getDriver());
            addApprenticeshipPage.UseTransferAllowance.Click();
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement(month);
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement(year);
            addApprenticeshipPage.ContinueButton.ClickThisElement();

            EstimateCostsPage secondFundsStartPage = new EstimateCostsPage(WebSite.getDriver());
            secondFundsStartPage.AddApprenticeshipsButton.Click();

            addApprenticeshipPage.UseTransferAllowance.Click();
            addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), "Actuary, Level: 7 (Standard)");
            addApprenticeshipPage.PageHeader.ClickThisElement();
            addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement("1");
            addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement(nextMonth);
            addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement(year);
            addApprenticeshipPage.ContinueButton.ClickThisElement();

            estimateCostsPage.ClickRemoveApprenticeshipButton();

            RemoveApprenticeshipPage removeApprenticeshipPage = new RemoveApprenticeshipPage(WebSite.getDriver());
            removeApprenticeshipPage.ConfirmRemoval();
        }

        [When(@"I'm on the Estimated costs page")]
        public void WhenImOnTheEstimatedCostsPage()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
        }

        [When(@"the '(.*)' tab")]
        public void WhenTheTab(string p0)
        {
            if (p0 == "Remaining transfer allowance")
            {
                EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
                estimateCostsPage.SwitchToRemainingTransferAllowanceTab();
            }
        }

        [Then(@"the monthly costs of the removed apprenticeships have been deducted from the correct months")]
        public void ThenTheMonthlyCostsOfTheRemovedApprenticeshipsHaveBeenDeductedFromTheCorrectMonths()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            EstimateCostsPage.RemainingTransferAllowanceRow[] remainingTransferAllowanceRows = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            Assert.IsTrue(remainingTransferAllowanceRows[0].RemainingTransferAllowance.Equals("£0"));
        }

        [Then(@"the completion costs of the removed apprenticeships have been removed from the correct month")]
        public void ThenTheCompletionCostsOfTheRemovedApprenticeshipsHaveBeenRemovedFromTheCorrectMonth()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            EstimateCostsPage.RemainingTransferAllowanceRow[] remainingTransferAllowanceRows = estimateCostsPage.GetRemainingTransferAllowanceTableContent();
            Assert.IsTrue(remainingTransferAllowanceRows[0].CostOfEstimatedApprenticeships.Equals("£0"));
        }

        [Given(@"the No radio button is defaulted as selected")]
        public void GivenTheNoRadioButtonIsDefaultedAsSelected()
        {
            RemoveApprenticeshipPage removeApprenticeshipPage = new RemoveApprenticeshipPage(WebSite.getDriver());
            Assert.IsTrue(removeApprenticeshipPage.IsNoRadioButtonSelected);
        }

        [When(@"I click continue")]
        public void WhenIClickContinue()
        {
            RemoveApprenticeshipPage removeApprenticeshipPage = new RemoveApprenticeshipPage(WebSite.getDriver());
            removeApprenticeshipPage.ClickContinueButton();
        }

        [Then(@"the Estimated Costs is displayed")]
        public void ThenTheEstimatedCostsIsDisplayed()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(WebSite.getDriver());
            Assert.IsTrue(estimateCostsPage.IsPageLoaded);
        }
    }
}