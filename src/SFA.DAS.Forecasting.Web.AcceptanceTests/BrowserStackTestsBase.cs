using System;
using System.Configuration;
using OpenQA.Selenium;
using SFA.DAS.Forecasting.Web.Automation;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    public class BrowserStackTestsBase
    {
        public string BaseUrl => ConfigurationManager.AppSettings["WebSiteUrl"];

        public bool IsLocalhost => BaseUrl.Contains("localhost");

        public bool DoesPageTextContain(IWebDriver driver, string text) => driver.PageSource.Contains(text);

        public string CurrentUrl(IWebDriver driver) => driver.Url;

        public LoginPage NavigateToLoginPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BaseUrl);
            var homepage = new HomePage(driver);
            return homepage.MoveToLoginPage();
        }

        public AccountHomePage NavigateToAccountHomePage(IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("[title*='SAINSBURY']")).Click();
            return new AccountHomePage(driver);
        }

        public EstimateFundsStartPage NavigateToEstimateFundsStartPage(IWebDriver driver)
        {
            string currentUrl = new Uri(driver.Url).ToString();
            if (currentUrl.Contains("forecasting/projections"))
            {
                driver.FindElement(By.CssSelector("a[href*='estimations/start']")).Click();
            }
            if (currentUrl.Contains(""))
            {

            }
            else
            {
                throw new Exception();
            }
            return new EstimateFundsStartPage(driver);
        }

        public EstimateCostsPage NavigateToEstimageCostsPage(IWebDriver driver)
        {
            driver.FindElement(By.CssSelector(".button-start")).Click();
            return new EstimateCostsPage(driver);
        }

        public FundingProjectionPage NavigateToFundingProjectionPage()
        {
            throw new NotImplementedException();
            /*var currentUrl = new Uri(WebBrowserDriver.Url);
            var baseUrl = currentUrl.GetLeftPart(UriPartial.Authority);
            WebBrowserDriver.Navigate().GoToUrl($"{baseUrl}/accounts/{EmployerHash}/forecasting/projections");
            return new FundingProjectionPage(WebBrowserDriver);*/
        }

        public void ClickOnElement(IWebDriver driver, string locator)
        {
            driver.FindElement(By.CssSelector(locator)).Click();
        }
    }
}