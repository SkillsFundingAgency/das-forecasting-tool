using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;
using SFA.DAS.Forecasting.Web.Automation.Estimation;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class EstimateCostsPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h1[text()='Estimated costs']")] public IWebElement PageHeader { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Remove']")] public IWebElement RemoveApprenticeshipButton { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Edit']")] public IWebElement EditApprenticeshipButton { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Remaining transfer allowance']")] public IWebElement RemainingTransferAllowanceTabButton { get; set; }
        [FindsBy(How = How.CssSelector, Using = ".total *[data-label='Total Cost']")] public IWebElement TotalCostLabel { get; set; }
        [FindsBy(How = How.XPath, Using = "//*[contains(@href, 'apprenticeship/add')]")] public IWebElement AddApprenticeshipsButton { get; set; }


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
            var selector = "#tab-2 table thead th";
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

        public ApprenticeshipsTableRow[] GetApprenticeshipsTableContent()
        {
            var rowSelector = "#tab-2 table tbody tr";
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
    } 
}