using OpenQA.Selenium;
using Sfa.Automation.Framework.Selenium;
using System;
using System.IO;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class ForecastingWebSite : TestBase
    {
        public static ForecastingWebSite Current { get; private set; }
        public string EmployerHash { get; private set; }
        public Uri BaseUrl { get; private set; }
        public ForecastingWebSite(string webSiteUrl, Sfa.Automation.Framework.Enums.WebDriver webDriver)
        {
            if (webSiteUrl == null) throw new ArgumentNullException(nameof(webSiteUrl));
            BaseUrl = new Uri(webSiteUrl);
            CommonTestSetup(BaseUrl, true, webDriver);
            Current = this;
        }

        public bool IsLocalhost => BaseUrl.Host.Contains("localhost");

        public bool DoesPageTextContain(string text) => WebBrowserDriver.PageSource.Contains(text);

<<<<<<< HEAD
        public string CurrentUrl => WebBrowserDriver.Url;

        public void ClickOnElement(string locator)
        {
            WebBrowserDriver.FindElement(By.CssSelector(locator)).Click();
        }

=======
>>>>>>> 91bc7306d1caed700cafd29a268f3d5b3b1da673
        public void SetEmployeeHash(string hash)
        {
            EmployerHash = hash;
        }

        public LoginPage NavigateToLoginPage()
        {
            WebBrowserDriver.Navigate().GoToUrl(BaseUrl);
            var homepage = new HomePage(WebBrowserDriver);
            return homepage.MoveToLoginPage();
        }

        public FundingProjectionPage NavigateToFundingProjectionPage()
        {
            var currentUrl = new Uri(WebBrowserDriver.Url);
            var baseUrl = currentUrl.GetLeftPart(UriPartial.Authority);
            WebBrowserDriver.Navigate().GoToUrl($"{baseUrl}/accounts/{EmployerHash}/forecasting/projections");
            return new FundingProjectionPage(WebBrowserDriver);
        }

        public EstimateFundsStartPage NavigateToEstimateFundsStartPage()
        {
            string currentUrl = new Uri(WebBrowserDriver.Url).ToString();
            if (currentUrl.Contains("forecasting/projections"))
            {
                WebBrowserDriver.FindElement(By.CssSelector("a[href*='estimations/start']")).Click();
            }
            if(currentUrl.Contains(""))
            {
                
            }
            else
            {
                throw new Exception();
            }
            return new EstimateFundsStartPage(WebBrowserDriver);
        }

        public EstimateCostsPage NavigateToEstimageCostsPage()
        {
            WebBrowserDriver.FindElement(By.CssSelector(".button-start")).Click();
            return new EstimateCostsPage(WebBrowserDriver);
        }

        public AccountHomePage NavigateToAccountHomePage()
        {
            WebBrowserDriver.FindElement(By.CssSelector("[title*='SAINSBURY']")).Click();
            return new AccountHomePage(WebBrowserDriver);
        }

        public IWebDriver getDriver()
        {
            return WebBrowserDriver;
        }

        public T NavigateTo<T>() where T : ForecastingPage, new()
        {
            var page = Activator.CreateInstance(typeof(T), WebBrowserDriver) as T;
            return page;
        }

		public void Authenticate(string employerHash)
		{
			//TODO: probably login via finance dashbaord or manually add auth cookie
			EmployerHash = employerHash ?? throw new ArgumentNullException(nameof(employerHash));
			AddEmployerHashToUrl(employerHash);
		}

	    public void AddEmployerHashToUrl(string employerHash)
	    {
		    BaseUrl = BaseUrl.Combine(employerHash);
	    }

		public void Close()
        {
            WebBrowserDriver.Quit();
        }
    }
}