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
            EmployerHash = "MDDP87";
            Console.WriteLine("Employer hash: MDDP87");
        }

        [Given(@"I have logged into my Apprenticeship Account")]
        public void GivenIHaveLoggedIntoMyApprenticeshipAccount()
        {
            //ScenarioContext.Current.Pending();
            WebSite.Authenticate(EmployerHash);  //TODO: add employer details here
        }

        [When(@"I navigate to the Landing page of the Forecasting dashboard")]
        public void WhenINavigateToTheLandingPageOfTheForecastingPortal()
        {
            var dashboard = WebSite.NavigateToDashboard();
            Set(dashboard);
        }

        [Then(@"the dashboard should be displayed")]
        public void ThenTheDashboardShouldBeDisplayed()
        {
            var dashboard = Get<DashboardPage>();
            Assert.IsTrue(dashboard.IsCurrentPage);
        }
    }
}
