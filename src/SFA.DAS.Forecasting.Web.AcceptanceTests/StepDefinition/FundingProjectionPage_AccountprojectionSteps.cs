using Dapper;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using SFA.DAS.Forecasting.ReadModel.Projections;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Sfa.Automation.Framework.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using static SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.DownloadForecastBalanceSheetSteps;
using SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]

    public class FundingProjectionPage_AccountprojectionSteps : StepsBase
    {
        private string[] headers = {
            "Date",
            "Funds in",
            "Cost of training",
            "Completion payments",
            "Future funds"
        };


        [When(@"the Account projection is displayed")]
        public void WhenTheAccountProjectionIsDisplayed()
        {
            var page = Get<FundingProjectionPage>();
            Assert.IsTrue(page.AccountProjectionHeader.Displayed);
            Assert.IsTrue(page.AccountProjectionTable.Displayed);
        }
        
        [When(@"I have a negative balance in a forecast month")]
        public void WhenIHaveANegativeBalanceInAForecastMonth()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the Account projection has the correct columns")]
        public void ThenTheAccountProjectionHasTheCorrectColumns()
        {
            var page = Get<FundingProjectionPage>();
            var table = Get<List<TestAccountProjection>>();
            var pageHeaders = page.GetAccountProjectionHeaders();
            Assert.AreEqual(pageHeaders.Length, headers.Length);
            foreach(var header in pageHeaders)
            {   
                Assert.Contains(header, headers);
            }
        }
        
        [Then(@"the first month displayed is the next calendar month")]
        public void ThenTheFirstMonthDisplayedIsTheNextCalendarMonth()
        {
            var nextMonth = DateHelper.GetMonthString(DateTime.Now.Month + 1);
            var table = Get<List<TestAccountProjection>>();
            var firstRow = table[0];
            StringAssert.Contains(nextMonth, firstRow.Date);
            var page = Get<FundingProjectionPage>();
            var datePageValues = page.GetHeaderValues("Date");
            var firstDate = datePageValues[0];
            StringAssert.Contains(nextMonth, firstDate);
        }
        
        [Then(@"there are months up to '(.*)' displayed in the forecast")]
        public void ThenThereAreMonthsUpToAprilDisplayedInTheForecast(string p0)
        {
            var table = Get<List<TestAccountProjection>>();
            var startDate = table[0].Date;
            var endDate = p0;
            var requiredDateRange = DateHelper.BuildDateRange(startDate, endDate);
            var page = Get<FundingProjectionPage>();
            var datePageValues = page.GetHeaderValues("Date");
            foreach (var date in requiredDateRange)
            {
                Assert.Contains(date, datePageValues);
            }
        }

        [Then(@"the data is displayed correctly in each column")]
        public void ThenTheDataIsDisplayedCorrectlyInEachColumn()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the first month's levy credit is shown as the calculated levy value")]
        public void ThenTheFirstMonthSLevyCreditIsShownAsTheCalculatedLevyValue()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the first month's balance includes the levy credit value")]
        public void ThenTheFirstMonthSBalanceIncludesTheLevyCreditValue()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"completion payments are shown against the correct months")]
        public void ThenCompletionPaymentsAreShownAgainstTheCorrectMonths()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the first month's balance uses a levy credit value of £(.*)")]
        public void ThenTheFirstMonthSBalanceUsesALevyCreditValueOf(int p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the balance for that month is displayed correctly as £(.*)")]
        public void ThenTheBalanceForThatMonthIsDisplayedCorrectlyAs(int p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
