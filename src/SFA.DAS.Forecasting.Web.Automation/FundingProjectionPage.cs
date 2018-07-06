using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class FundingProjectionPage: ForecastingPage
    {
        [FindsBy(How = How.Id, Using= "projections_csvdownload")]
        public IWebElement DownloadProjectionsCSVButton { get; set; }

        [FindsBy(How = How.Id, Using = "apprenticeship_csvdownload")]
        public IWebElement DownloadApprenticeshipCSVButton { get; set; }

        [FindsBy(How = How.XPath, Using = "//h1[contains(text(), 'Funding projection')]")]
        public IWebElement AccountProjectionHeader { get; set; }

        [FindsBy(How = How.ClassName, Using = "balancesheet")]
        public IList<IWebElement> AccountProjectionTables { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".pending-completion-payments span")]
        public IWebElement PendingCompletionPayments { get; set; }

        public FundingProjectionPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public override string UrlFragment => "forecasting";
        public override bool IsCurrentPage => DownloadProjectionsCSVButton?.Displayed ?? false;

        public string[] GetAccountProjectionHeaders()
        {
            var elements = Driver.FindElements(By.CssSelector(".balancesheet:first-of-type thead tr:last-child th"));
            return Filter(elements);
        }

        public string[] GetHeaderValues(string headerName)
        {
            var headers = GetAccountProjectionHeaders();
            var index = Array.IndexOf(headers, headerName);
            var elements = Driver.FindElements(By.CssSelector($".balancesheet tbody td:nth-child({index + 1})"));
            return elements
                .Select((element) => element.Text)
                .ToArray();
        }

        private string[] Filter(ReadOnlyCollection<IWebElement> elements)
        {
            return elements
                .Select((element) => element.Text)
                .Select(m => m.Replace("\r", " "))
                .Select(m => m.Replace("\n", ""))
                .Select(m => m.Replace("info", ""))
                .Select(m => m.Replace("available soon", ""))
                .Select(m => m.Trim())
                .ToArray();
        }
    } 
}