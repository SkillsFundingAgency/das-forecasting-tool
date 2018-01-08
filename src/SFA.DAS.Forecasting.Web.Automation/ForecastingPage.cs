using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Sfa.Automation.Framework.Constants;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public abstract class ForecastingPage: BasePage
    {
        public abstract string UrlFragment { get; }

        protected ForecastingPage(IWebDriver webDriver) : base(webDriver)
        {
            
        }

        protected void WaitForLoadingToFinish()
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            wait.Until(driver => !driver.FindElement(By.Id("Loading")).Displayed);
        }

        public abstract bool IsCurrentPage { get; }
    }
}