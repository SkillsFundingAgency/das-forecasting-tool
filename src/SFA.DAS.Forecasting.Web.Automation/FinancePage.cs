using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class FinancePage: BasePage
    {
        [FindsBy(How = How.CssSelector, Using = "*[title='Funding projection']")] public IWebElement FundingProjectionButton { get; set; }

        public FinancePage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public FundingProjectionPage OpenFundingProjection()
        {
            FundingProjectionButton.ClickThisElement();
            return new FundingProjectionPage(Driver);
        }
    } 
}