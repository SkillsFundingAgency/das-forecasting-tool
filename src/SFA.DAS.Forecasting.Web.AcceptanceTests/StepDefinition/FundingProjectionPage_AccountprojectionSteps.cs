using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using static SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition.DownloadForecastBalanceSheetSteps;
using SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers;
using System.Globalization;
using System.Transactions;
using SFA.DAS.Forecasting.Data;
using System.Linq;
using OpenQA.Selenium;
using StructureMap;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class FundingProjectionPage_AccountprojectionSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;
        protected IContainer NestedContainer { get; set; }

        public FundingProjectionPage_AccountprojectionSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        private string[] expectedHeaders = {
            "Date",
            "Cost of training",
            "Completion payments",
            "Expired funds",
            "Funds in",
            "Your contribution (10%)",
            "Government contribution (90%)",
            "Balance"
        };

        private string[] expectedHeadersWithoutCoInvestment = {
            "Date",
            "Cost of training",
            "Completion payments",
            "Expired funds",
            "Funds in",
            "Balance"
        };

        private Dictionary<string, Func<string, string>> dataMappers = new Dictionary<string, Func<string, string>>
        {
            { "Date",  value => value },
            { "CostOfTraining",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "CompletionPayments",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "ExpiredFunds",  value => "–" },
            { "FundsIn",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "YourContribution",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "GovernmentContribution",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
            { "Balance",  value => StringHelper.CurrencyConverter(Decimal.Parse(value)) },
        };

        protected IForecastingDataContext ForecastingDataContext => NestedContainer.GetInstance<IForecastingDataContext>();

        [When(@"the Account projection is displayed")]
        public void WhenTheAccountProjectionIsDisplayed()
        {
            FundingProjectionPage page = new FundingProjectionPage(_driver);

            var firstProjectionTable = page.AccountProjectionTables.FirstOrDefault();
            Assert.IsTrue(page.AccountProjectionHeader.Displayed, "ERROR:The account projection header is not visible");
            Assert.IsTrue(firstProjectionTable.Displayed, "ERROR:The account projection table is not visible");

        }

        [When(@"I have a negative balance in a forecast month")]
        public void WhenIHaveANegativeBalanceInAForecastMonth()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the Account projection has the correct columns")]
        public void ThenTheAccountProjectionHasTheCorrectColumns()
        {
            FundingProjectionPage page = new FundingProjectionPage(_driver);
            List<TestAccountProjection> table = new List<TestAccountProjection>();
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
            FundingProjectionPage page = new FundingProjectionPage(_driver);
            List<TestAccountProjection> table = new List<TestAccountProjection>();
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
            // this step became invalid. the website shown may instead of jun as a first row date

            var nextMonthDate = DateTime.Now.AddMonths(1);
            var nextMonth = DateHelper.GetMonthString(nextMonthDate.Month);
            FundingProjectionPage page = new FundingProjectionPage(_driver);
            var datePageValues = page.GetHeaderValues("Date");
            var firstDate = datePageValues[0];
            StringAssert.Contains(nextMonth, firstDate, "ERROR:The first month displayed is not the next calender month");
        }

        [Then(@"there are months up to '(.*)' displayed in the forecast")]
        public void ThenThereAreMonthsUpToAprilDisplayedInTheForecast(string p0)
        {
            FundingProjectionPage page = new FundingProjectionPage(_driver);
            var datePageValues = page.GetHeaderValues("Date");

            List<TestAccountProjection> table = new List<TestAccountProjection>();
            var startDate = table[1].Date;
            var endDate = p0;
            var requiredDateRange = DateHelper.BuildDateRange(datePageValues.FirstOrDefault(), endDate);
            foreach (var date in requiredDateRange)
            {
                Assert.Contains(date, datePageValues);
            }
        }

        [Then(@"the data is displayed correctly in each column")]
        public void ThenTheDataIsDisplayedCorrectlyInEachColumn()
        {
            List<TestAccountProjection> table = new List<TestAccountProjection>();
            FundingProjectionPage page = new FundingProjectionPage(_driver);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            foreach (var headerName in expectedHeaders)
            {
                var datePageValues = page.GetHeaderValues(headerName);
                for (int i = 0; i < datePageValues.Length; i++)
                {
                    var row = table[i +1];
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

        [Given(@"I have completion payments of £ (.*) on commitments without stop date")]
        public void GivenIHaveCompletionPaymentsOfOnCommitmentsWithoutStopDate(int overdueCompletionPayments)
        {
            StoreUnallocatedCompletionPayments(overdueCompletionPayments);
        }

        [Then(@"I see Pending completion payments with the amount of £ (.*)")]
        public void ThenISeePendingCompletionPaymentsWithTheAmountOf(string overdueCompletionPayments)
        {
            FundingProjectionPage page = new FundingProjectionPage(_driver);
            Assert.AreEqual($"£{overdueCompletionPayments}", page.PendingCompletionPayments.Text);
        }

        private void StoreUnallocatedCompletionPayments(int v)
        {
            Config config = new Config();
            var employerAccountId = long.Parse(config.EmployerAccountID);
            var balance = ForecastingDataContext.Balances.FirstOrDefault(m => m.EmployerAccountId == employerAccountId);

            if (balance == null)
            {
                balance = new Models.Balance.BalanceModel
                {
                    EmployerAccountId = employerAccountId,
                    Amount = 5001,
                    TransferAllowance = 501,
                    RemainingTransferBalance = 501,
                    BalancePeriod = DateTime.UtcNow,
                    ReceivedDate = DateTime.UtcNow,
                    UnallocatedCompletionPayments = 2401
                };
                ForecastingDataContext.Balances.Add(balance);
            }
            else
            {
                balance.UnallocatedCompletionPayments = 2401;
            }
            
            ForecastingDataContext.SaveChanges();
        }
    }
}
