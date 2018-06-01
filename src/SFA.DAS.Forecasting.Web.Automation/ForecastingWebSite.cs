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
            var currentUrl = new Uri(WebBrowserDriver.Url);
            // TODO its a workaround to move on the page until estimations link implemented on employer
            var baseUrl = currentUrl.GetLeftPart(UriPartial.Authority);
            WebBrowserDriver.Navigate().GoToUrl($"{baseUrl}/accounts/{EmployerHash}/forecasting/estimations/start-transfer");
            return new EstimateFundsStartPage(WebBrowserDriver);
        }

        public EstimateCostsPage NavigateToEstimageCostsPage()
        {
            var currentUrl = new Uri(WebBrowserDriver.Url);
            var baseUrl = currentUrl.GetLeftPart(UriPartial.Authority);
            WebBrowserDriver.Navigate().GoToUrl($"{baseUrl}/accounts/{EmployerHash}/forecasting/estimations/default");
            return new EstimateCostsPage(WebBrowserDriver);
        }

        public AccountHomePage NavigateToAccountHomePage()
        {
            var currentUrl = new Uri(WebBrowserDriver.Url);
            var baseUrl = currentUrl.GetLeftPart(UriPartial.Authority);
            WebBrowserDriver.Navigate().GoToUrl($"{baseUrl}/accounts/{EmployerHash}/teams");
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