using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class FundingProjectionPage: ForecastingPage
    {
        [FindsBy(How = How.CssSelector, Using= ".related .form-group a")]
        public IWebElement DownloadCSVButton { get; set; }

        public FundingProjectionPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public override string UrlFragment => "forecasting";
        public override bool IsCurrentPage => DownloadCSVButton?.Displayed ?? false;
    }
}