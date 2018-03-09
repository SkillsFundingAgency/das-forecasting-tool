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

        public LoginPage NavigateToLoginPage()
        {
            WebBrowserDriver.Navigate().GoToUrl(BaseUrl.Combine("forecasting"));
            return new LoginPage(WebBrowserDriver);
        }

        public DashboardPage NavigateToDashboard()
        {
            WebBrowserDriver.Navigate().GoToUrl(BaseUrl.Combine("forecasting/balance"));
            return new DashboardPage(WebBrowserDriver);
        }

        public FundingProjectionPage NavigateToFundingProjectionPage()
        {
            WebBrowserDriver.Navigate().GoToUrl(BaseUrl.Combine("forecasting"));
            return new FundingProjectionPage(WebBrowserDriver);
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