using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using Sfa.Automation.Framework.Constants;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class EstimateFundsStartPage: BasePage
    {
        [FindsBy(How = How.ClassName, Using = "button-start")] private IWebElement StartButton { get; set; }

        public EstimateFundsStartPage(IWebDriver webDriver) : base(webDriver)
        {

        }
    }
}