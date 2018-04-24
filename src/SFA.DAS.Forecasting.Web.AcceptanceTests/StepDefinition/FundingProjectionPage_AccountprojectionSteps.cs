using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using static SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.DownloadForecastBalanceSheetSteps;
using SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers;
using System.Globalization;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]

    public class FundingProjectionPage_AccountprojectionSteps : StepsBase
    {
        private string[] expectedHeaders = {
            "Date",
            "Funds in",
            "Cost of training",
            "Completion payments",
            "Your contribution (10%)",
            "Government contribution (90%)",
            "Future funds"
        };

        private string[] expectedHeadersWithoutCoInvestment = {
            "Date",
            "Funds in",
            "Cost of training",
            "Completion payments",
            "Future funds"
        };

        private Dictionary<string, Func<string, string>> dataMappers = new Dictionary<string, Func<string, string>>
        {
            { "Date",  value => value },
            { "FundsIn",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "CostOfTraining",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "CompletionPayments",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "FutureFunds",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "YourContribution",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "GovernmentContribution",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) }
        };


        [When(@"the Account projection is displayed")]
        public void WhenTheAccountProjectionIsDisplayed()
        {
            var page = Get<FundingProjectionPage>();
            //Thread.Sleep(10000);
            Assert.IsTrue(page.AccountProjectionHeader.Displayed, "ERROR:The account projection header is not visible");
            Assert.IsTrue(page.AccountProjectionTable.Displayed, "ERROR:The account projection table is not visible");
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
            Assert.AreEqual(expectedHeaders.Length, pageHeaders.Length);
            foreach (var header in pageHeaders)
            {
                Assert.Contains(header, expectedHeaders, "ERROR:The account projection does not have the correct cloumns");
            }
        }

        [Then(@"the Account projection has the correct columns without Co-Investment")]
        public void ThenTheAccountProjectionHasTheCorrectColumnsWithoutCoInvestment()
        {
            var page = Get<FundingProjectionPage>();
            var table = Get<List<TestAccountProjection>>();
            var pageHeaders = page.GetAccountProjectionHeaders();
            Assert.AreEqual(expectedHeadersWithoutCoInvestment.Length, pageHeaders.Length);
            foreach (var header in pageHeaders)
            {
                Assert.Contains(header, expectedHeadersWithoutCoInvestment, "ERROR:The account projection does not have the correct cloumns");
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
            StringAssert.Contains(nextMonth, firstDate, "ERROR:The first month displayed is not the next calender month");
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
            var table = Get<List<TestAccountProjection>>();
            var page = Get<FundingProjectionPage>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            foreach (var headerName in expectedHeaders)
            {
                var datePageValues = page.GetHeaderValues(headerName);
                Assert.AreEqual(datePageValues.Length, table.Count);
                for (int i = 0; i < datePageValues.Length; i++)
                {
                    var row = table[i];
                    var propertyName = textInfo.ToTitleCase(headerName).Replace(" ", String.Empty);
                    var value = row
                        .GetType()
                        .GetProperty(propertyName)
                        .GetValue(row, null);
                    var formattedValue = dataMappers[propertyName](value.ToString());
                    Assert.AreEqual(formattedValue, datePageValues[i], "ERROR:Data is not displayed correctly");
                }
            }
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
