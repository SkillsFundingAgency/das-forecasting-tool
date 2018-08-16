using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;

namespace SFA.DAS.Forecasting.Web.Automation.Estimation
{
    public class EditApprenticeshipsPage : AddEditApprenticeshipPage
    {
        [FindsBy(How = How.CssSelector, Using = "button[type='submit']")] public IWebElement ContinueButton { get; set; }

        public EditApprenticeshipsPage(IWebDriver webDriver) : base(webDriver)
        {
        }
    }
}
