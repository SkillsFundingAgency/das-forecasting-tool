using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class ForecastingDashboardSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public ForecastingDashboardSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        [Given(@"that I am an employer")]
        public void GivenThatIAmAnEmployer()
        {
            //TODO: replace with employer credentials
            //EmployerHash = "M6PKPG";
            /*string EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            string EmployeePassword = "Dell1507";
            WebSite.SetEmployeeHash(EmployerHash);
            Console.WriteLine("Employer hash: M6PKPG");*/
        }

		[Given(@"I have logged into my Apprenticeship Account")]
		public void GivenIHaveLoggedIntoMyApprenticeshipAccount()
		{
		    string EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
		    string EmployeePassword = "Dell1507";

            if (!IsLocalhost)
            {
                LoginPage loginPage = NavigateToLoginPage(_driver);
                loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);
            }
		}

	    [Given(@"I am not logged into my Apprenticeship Account")]
	    public void GivenIAmNotLoggedIntoMyApprenticeshipAccount()
	    {
            //obsoleted logic
		    //WebSite.AddEmployerHashToUrl(EmployerHash);
	    }

		[When(@"I navigate to the Landing page of the Forecasting dashboard")]
        public void WhenINavigateToTheLandingPageOfTheForecastingPortal()
        {
            if (!IsLocalhost)
            {
                AccountHomePage accountHomepage = NavigateToAccountHomePage(_driver);
                FinancePage financePage = accountHomepage.OpenFinance();
                FundingProjectionPage page = financePage.OpenFundingProjection();
            }
            else
            {
                var page = NavigateToFundingProjectionPage();
            }
        }

        [Then(@"the dashboard should be displayed")]
        public void ThenTheDashboardShouldBeDisplayed()
        {
            FundingProjectionPage page = new FundingProjectionPage(_driver);          
            Assert.IsTrue(page.AccountProjectionHeader.Displayed, "ERROR:The account projection header is not visible");
            Assert.IsTrue(page.AccountProjectionTables.FirstOrDefault().Displayed, "ERROR:The account projection table is not visible");
        }
    }
}
