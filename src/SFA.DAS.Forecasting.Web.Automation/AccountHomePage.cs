using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class AccountHomePage: BasePage
    {
        [FindsBy(How = How.XPath, Using = "//a[text()='Finance']")] public IWebElement FinanceButton { get; set; }

        public AccountHomePage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public FinancePage OpenFinance()
        {
            FinanceButton.ClickThisElement();
            return new FinancePage(Driver);
        }
    } 
}