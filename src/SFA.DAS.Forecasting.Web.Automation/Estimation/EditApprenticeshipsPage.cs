using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;

namespace SFA.DAS.Forecasting.Web.Automation.Estimation
{
    public class EditApprenticeshipsPage : BasePage
    {
        [FindsBy(How = How.CssSelector, Using = "button[type='submit']")] public IWebElement ContinueButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".heading-xlarge")]
        public IWebElement Heading { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#no-of-app")]
        public IWebElement NumberOfMonthsInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#total-funding-cost")] 
        public IWebElement TotalCostInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#startDateYear")]
        public IWebElement YearInput { get; set; }

        public EditApprenticeshipsPage(IWebDriver webDriver) : base(webDriver)
        {
        }
    }
}
