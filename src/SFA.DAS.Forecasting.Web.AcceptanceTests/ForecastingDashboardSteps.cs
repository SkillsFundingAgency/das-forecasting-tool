using System;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class ForecastingDashboardSteps: StepsBase
    {
        [Given(@"that I am an employer")]
        public void GivenThatIAmAnEmployer()
        {
            //TODO: replace with employer credentials
            EmployerHash = "M6PKPG";
            EmployeeLogin = "dele.odusanya@lynkmiigroup.com";
            EmployeePassword = "Dell1507";
            WebSite.SetEmployeeHash(EmployerHash);
            Console.WriteLine("Employer hash: M6PKPG");
        }

		[Given(@"I have logged into my Apprenticeship Account")]
		public void GivenIHaveLoggedIntoMyApprenticeshipAccount()
		{
            //ScenarioContext.Current.Pending();
            //WebSite.Authenticate(EmployerHash);  //TODO: add employer details here
            var loginPage = WebSite.NavigateToLoginPage();
            loginPage.LoginAsUser(EmployeeLogin, EmployeePassword);
		}

	    [Given(@"I am not logged into my Apprenticeship Account")]
	    public void GivenIAmNotLoggedIntoMyApprenticeshipAccount()
	    {
		    WebSite.AddEmployerHashToUrl(EmployerHash);
	    }

		[When(@"I navigate to the Landing page of the Forecasting dashboard")]
        public void WhenINavigateToTheLandingPageOfTheForecastingPortal()
        {
            var accountHomepage = WebSite.NavigateToAccountHomePage();
            var financePage = accountHomepage.OpenFinance();
            var page = financePage.OpenFundingProjection();
            Set(page);            
        }

        [Then(@"the dashboard should be displayed")]
        public void ThenTheDashboardShouldBeDisplayed()
        {
            var page = Get<FundingProjectionPage>();            
            Assert.IsTrue(page.AccountProjectionHeader.Displayed, "ERROR:The account projection header is not visible");
            Assert.IsTrue(page.AccountProjectionTable.Displayed, "ERROR:The account projection table is not visible");

        }
    }
}
