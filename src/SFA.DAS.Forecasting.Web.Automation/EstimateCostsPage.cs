using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;
using SFA.DAS.Forecasting.Web.Automation.Estimation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class EstimateCostsPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h1[text()='Estimated costs']")]
        public IWebElement PageHeader { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Remove']")]
        public IWebElement RemoveApprenticeshipButton { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Edit']")]
        public IWebElement EditApprenticeshipButton { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Remaining transfer allowance']")]
        public IWebElement RemainingTransferAllowanceTabButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".total *[data-label='Total Cost']")]
        public IWebElement TotalCostLabel { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[contains(@href, 'apprenticeship/add')]")]
        public IWebElement AddApprenticeshipsButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#tab-apprenticeships-added > a"), CacheLookup]
        private IWebElement _apprenticeshipsAddedTabLink;

        [FindsBy(How = How.Id, Using = "tab-apprenticeships-added"), CacheLookup]
        private IWebElement _apprenticeshipsAddedTab;

        [FindsBy(How = How.CssSelector, Using = "#tab-remaining-transfer-allowance > a"), CacheLookup]
        private IWebElement _remainingTransferAllowanceTabLink;

        [FindsBy(How = How.CssSelector, Using = ".column-full>details>summary"), CacheLookup]
        private IWebElement _whatDoesThisTableShowLink;

        [FindsBy(How = How.CssSelector, Using = "#details-content-0>p"), CacheLookup]
        private IList<IWebElement> _whatDoesThisTableShowText;

        [FindsBy(How = How.CssSelector, Using = ".panel.panel-width-thin>p")]
        private IWebElement _youHaveNotSelectedAnyApprenticeshipsMessage;

        public EstimateCostsPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool IsApprenticeshipsTableVisible()
        {
            var aprenticeshipsAddedTabButtonSelector = "//a[text()='Apprenticeships added']";
            PageHeader.WaitForElementIsVisible();
            var tabs = Driver.FindElements(By.XPath(aprenticeshipsAddedTabButtonSelector));
            return tabs.Count > 0;
        }

        public void SwitchToApprenticeshipsAddedTab()
        {
            _apprenticeshipsAddedTabLink.Click();
        }

        public bool IsApprenticeshipsAddedTabActive => _apprenticeshipsAddedTab.GetAttribute("class").Contains("current");

        public void SwitchToRemainingTransferAllowanceTab()
        {
            _remainingTransferAllowanceTabLink.Click();
        }

        public bool IsRemainingTransferAllowanceTableDisplayed
        {
            get
            {
                if (_remainingTransferAllowanceTabLink != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsYouHaveNotSelectedAnyApprenticeshipsMessageDisplayed =>
            _youHaveNotSelectedAnyApprenticeshipsMessage.Displayed;

        public void ClickOnWhatDoesThisTableShowLink()
        {
            _whatDoesThisTableShowLink.Click();
        }

        public bool IsTextWhatDoesThisTableShowDisplayed
        {
            get
            {
                if (_whatDoesThisTableShowText != null)
                {
                    return true;
                }
                return false;
            }
        }

        public IList<string> WhatDoesThisTableShowText
        {
            get
            {
                IList<string> whatDoesThisTableShowText = new List<string>();
                foreach (var text in _whatDoesThisTableShowText)
                {
                    whatDoesThisTableShowText.Add(text.Text);
                }
                return whatDoesThisTableShowText;
            }
        }

        public void RemoveFirstApprenticeship()
        {
            RemoveApprenticeshipButton.ClickThisElement();
            var removalPage = new RemoveApprenticeshipPage(Driver);
            removalPage.ConfirmRemoval();
        }

        public AddApprenticeshipsToEstimateCostPage AddApprenticeships()
        {
            this.AddApprenticeshipsButton.ClickThisElement();
            return new AddApprenticeshipsToEstimateCostPage(Driver);
        }

        public EditApprenticeshipsPage EditApprenticeships()
        {
            this.EditApprenticeshipButton.ClickThisElement();
            return new EditApprenticeshipsPage(Driver);
        }

        public string[] GetApprenticeshipsAddedTableHeaders()
        {
            var selector = "#apprenticeships-added>table>thead>tr>th";
            var headerElements = Driver.FindElements(By.CssSelector(selector));
            return headerElements
                .Select((element) => element.Text)
                .Where((text) => text != "")
                .ToArray();
        }

        private string GetTableResultElement(string rowSelector, int rowNumber, string dataLabel)
        {
            var selector = $"{rowSelector}:nth-child({rowNumber}) td[data-label='{dataLabel}']";
            return Driver
                .FindElement(By.CssSelector(selector))
                .Text;
        }

        public bool VerifyThatThereIsHighlightingOfAnyRow
        {
            get
            {
                var rowsSelector = "#remaining-transfer-allowance>table>tbody>tr";
                var rows = Driver.FindElements(By.CssSelector(rowsSelector));
                foreach (var row in rows)
                {
                    if (row.GetAttribute("Class").Contains("error-row"))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public RemainingTransferAllowanceRow[] GetRemainingTransferAllowanceTableContent()
        {
            var rowSelector = "#remaining-transfer-allowance>table:first-of-type>tbody>tr";
            var rowElements = Driver.FindElements(By.CssSelector(rowSelector));
            var resultRows = new RemainingTransferAllowanceRow[rowElements.Count];
            for (int i = 0; i < rowElements.Count; i++)
            {
                var resultRow = new RemainingTransferAllowanceRow();
                resultRow.Date = GetRemainingTransferAllowanceTableResultElement(rowSelector, i + 1, 1);
                resultRow.YourCurrentTransferCosts = GetRemainingTransferAllowanceTableResultElement(rowSelector, i + 1, 2);
                resultRow.CostOfEstimatedApprenticeships = GetRemainingTransferAllowanceTableResultElement(rowSelector, i + 1, 3);
                resultRow.RemainingTransferAllowance = GetRemainingTransferAllowanceTableResultElement(rowSelector, i + 1, 4);
                resultRows[i] = resultRow;
            }
            return resultRows;
        }

        private string GetRemainingTransferAllowanceTableResultElement(string rowSelector, int rowNumber, int index)
        {
            var selector = $"{rowSelector}:nth-child({rowNumber})>td:nth-child({index})";
            return Driver
                .FindElement(By.CssSelector(selector))
                .Text;
        }


        public ApprenticeshipsTableRow[] GetApprenticeshipsTableContent()
        {
            var rowSelector = "#apprenticeships-added>table>tbody>tr";
            var rowElements = Driver.FindElements(By.CssSelector(rowSelector));
            var resultRows = new ApprenticeshipsTableRow[rowElements.Count];
            for(int i = 0; i < rowElements.Count; i++)
            {
                var resultRow = new ApprenticeshipsTableRow();
                resultRow.Apprenticeship = GetTableResultElement(rowSelector, i+1, "Apprenticeship");
                resultRow.NumberOfApprentices = GetTableResultElement(rowSelector, i+1, "Number of apprentices");
                resultRow.StartDate = GetTableResultElement(rowSelector, i+1, "Start Date");
                resultRow.TotalCost = GetTableResultElement(rowSelector, i+1, "Total Cost");
                resultRow.NumberOfMonthlyPayments = GetTableResultElement(rowSelector, i+1, "Number of monthly payments");
                resultRow.MonthlyPayment = GetTableResultElement(rowSelector, i+1, "Monthly payment");
                resultRow.CompletionPayment = GetTableResultElement(rowSelector, i+1, "Completion payment");
                resultRows[i] = resultRow;
            }
            return resultRows;
        }

        public class ApprenticeshipsTableRow
        {
            public string Apprenticeship { get; set; }
            public string NumberOfApprentices { get; set; }
            public string StartDate { get; set; }
            public string TotalCost { get; set; }
            public string NumberOfMonthlyPayments { get; set; }
            public string MonthlyPayment { get; set; }
            public string CompletionPayment { get; set; }
        }

        public class RemainingTransferAllowanceRow
        {
            public string Date { get; set; }
            public string YourCurrentTransferCosts  { get; set; }
            public string CostOfEstimatedApprenticeships  { get; set; }
            public string RemainingTransferAllowance { get; set; }
        }
    }
}