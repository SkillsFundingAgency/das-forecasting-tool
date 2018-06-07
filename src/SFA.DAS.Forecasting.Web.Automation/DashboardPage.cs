using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class DashboardPage: ForecastingPage
    {
        [FindsBy(How = How.CssSelector, Using= "h1.heading-xlarge")]
        public IWebElement Heading { get; set; }

        public DashboardPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public override string UrlFragment => "forecasting/projections";
        public override bool IsCurrentPage => Heading?.Displayed ?? false;
    }
}