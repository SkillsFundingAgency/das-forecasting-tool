using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class FundingProjectionPage: ForecastingPage
    {
        [FindsBy(How = How.CssSelector, Using= ".related .form-group a")]
        public IWebElement DownloadCSVButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = "h4.heading-large")]
        public IWebElement AccountProjectionHeader { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#balancesheet")]
        public IWebElement AccountProjectionTable { get; set; }

        public FundingProjectionPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public override string UrlFragment => "forecasting";
        public override bool IsCurrentPage => DownloadCSVButton?.Displayed ?? false;

        public string[] GetAccountProjectionHeaders()
        {
            var headersElements = Driver.FindElements(By.CssSelector("#balancesheet thead tr:last-child th"));
            return headersElements
                .Select((element) => element.Text)
                .ToArray();
        }

        public string[] GetHeaderValues(string headerName)
        {
            var headers = GetAccountProjectionHeaders();
            var index = Array.IndexOf(headers, headerName);
            var elements = Driver.FindElements(By.CssSelector($"#balancesheet tbody td:nth-child({index + 1})"));
            return elements
                .Select((element) => element.Text)
                .ToArray();
        }
    } 
}