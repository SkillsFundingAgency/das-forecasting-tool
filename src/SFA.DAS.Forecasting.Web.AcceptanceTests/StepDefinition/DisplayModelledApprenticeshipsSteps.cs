using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class DisplayModelledApprenticeshipsSteps : BrowserStackTestsBase
    {
        private IWebDriver _driver;
        readonly BrowserStackDriver _bsDriver;

        public DisplayModelledApprenticeshipsSteps()
        {
            _bsDriver = (BrowserStackDriver)ScenarioContext.Current["bsDriver"];
            _driver = _bsDriver.GetExisting();
            if (_driver == null)
            {
                _driver = _bsDriver.Init("single", "bs");
            }
        }

        [Given(@"that I have added the following apprenticeships")]
        public void GivenThatIHaveAddedTheFollowingApprenticeships(Table table)
        {
            var apprenticeships = table.CreateSet<TestApprenticeship>().ToList();

            var estimateCostsPage = NavigateToEstimageCostsPage(_driver);
            var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
            while (isAnyapprenticeshipExist)
            {
                estimateCostsPage.RemoveFirstApprenticeship();
                isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
            }
            foreach(var apprenticeship in apprenticeships)
            {
                AddApprenticeshipsToEstimateCostPage addApprenticeshipPage = new AddApprenticeshipsToEstimateCostPage(_driver);
                if (!_driver.Url.Contains("forecasting/estimations/default/apprenticeship/add"))
                {
                    addApprenticeshipPage = estimateCostsPage.AddApprenticeships();
                }
                
                addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(_driver, apprenticeship.Apprenticeship);
                addApprenticeshipPage.PageHeader.ClickThisElement();
                addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement(apprenticeship.NumberOfApprentices);
                addApprenticeshipPage.NumberOfMonthsInput.Clear();
                addApprenticeshipPage.NumberOfMonthsInput.EnterTextInThisElement(apprenticeship.NumberOfMonths);
                addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement(apprenticeship.StartDateMonth);
                addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement(apprenticeship.StartDateYear);
                addApprenticeshipPage.TotalCostInput.Clear();
                addApprenticeshipPage.TotalCostInput.EnterTextInThisElement(apprenticeship.TotalCost);
                addApprenticeshipPage.ContinueButton.ClickThisElement();
            }
        }
        
        [When(@"the modelled apprenticeships page is displayed")]
        public void WhenTheModelledApprenticeshipsPageIsDisplayed()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            Assert.IsTrue(estimateCostsPage.PageHeader.Displayed);
        }
        
        [Then(@"the column headings are displayed")]
        public void ThenTheColumnHeadingsAreDisplayed()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var tableHeaders = estimateCostsPage.GetApprenticeshipsAddedTableHeaders();
            var expectedHeaders = new String[] {
                "Apprenticeship\r\nLevel",
                "Number of apprentices",
                "Start date",
                "Total Cost",
                "Number of monthly payments",
                "Monthly payment",
                "Completion payment",
                "Transfer estimate\r\ninfo"
            };
            Assert.AreEqual(expectedHeaders, tableHeaders);
        }

        [Then(@"each added apprenticeship is displayed in a separate row")]
        public void ThenEachAddedApprenticeshipIsDisplayedInASeparateRow()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var resultRows = estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.AreEqual(3, resultRows.Count);
        }
        
        [Then(@"the apprenticeship with the earliest start date is shown first")]
        public void ThenTheApprenticeshipWithTheEarliestStartDateIsShownFirst()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var resultRows = estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.IsTrue(DateTime.Parse(resultRows[0].StartDate) > DateTime.Parse(resultRows[1].StartDate), "The apprenticeships in incorrect order");
        }
        
        [Then(@"the other apprenticeships are in order of start date")]
        public void ThenTheOtherApprenticeshipsAreInOrderOfStartDate()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            var resultRows = estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.IsTrue(DateTime.Parse(resultRows[1].StartDate) < DateTime.Parse(resultRows[2].StartDate), "The other apprenticeships are not in order of start date");
        }
        
        [Then(@"the details against each apprenticeship match what was entered")]
        public void ThenTheDetailsAgainstEachApprenticeshipMatchWhatWasEntered()
        {
            EstimateCostsPage estimateCostsPage = new EstimateCostsPage(_driver);
            List<EstimateCostsPage.ApprenticeshipsTableRow> resultRows = estimateCostsPage.GetApprenticeshipsTableContent();
            List<EstimateCostsPage.ApprenticeshipsTableRow> expectedResultRows = new List<EstimateCostsPage.ApprenticeshipsTableRow>();
            var resultRow = new EstimateCostsPage.ApprenticeshipsTableRow();
            resultRows[0].Apprenticeship = $"Advanced butcher{Environment.NewLine}3";
            resultRow.Apprenticeship = $"Advanced butcher{Environment.NewLine}3";
            resultRow.CompletionPayment = "£2,400";
            resultRow.MonthlyPayment = "£800";
            resultRow.NumberOfApprentices = "1";
            resultRow.NumberOfMonthlyPayments = "12";
            resultRow.StartDate = "Mar 2019";
            resultRow.TotalCost = "£12,000";
            expectedResultRows.Add(resultRow);

            var resultRow1 = new EstimateCostsPage.ApprenticeshipsTableRow();
            resultRows[1].Apprenticeship = $"Baker{Environment.NewLine}2";
            resultRow1.Apprenticeship = $"Baker{Environment.NewLine}2";
            resultRow1.CompletionPayment = "£5,400";
            resultRow1.MonthlyPayment = "£1,440";
            resultRow1.NumberOfApprentices = "3";
            resultRow1.NumberOfMonthlyPayments = "15";
            resultRow1.StartDate = "Dec 2018";
            resultRow1.TotalCost = "£27,000";
            expectedResultRows.Add(resultRow1);

            var resultRow2 = new EstimateCostsPage.ApprenticeshipsTableRow();
            resultRows[2].Apprenticeship = $"Network engineer{Environment.NewLine}4";
            resultRow2.Apprenticeship = $"Network engineer{Environment.NewLine}4";
            resultRow2.CompletionPayment = "£7,200";
            resultRow2.MonthlyPayment = "£1,200";
            resultRow2.NumberOfApprentices = "2";
            resultRow2.NumberOfMonthlyPayments = "24";
            resultRow2.StartDate = "Dec 2020";
            resultRow2.TotalCost = "£36,000";
            expectedResultRows.Add(resultRow2);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(resultRows[i].Apprenticeship, expectedResultRows[i].Apprenticeship);
                Assert.AreEqual(resultRows[i].CompletionPayment, expectedResultRows[i].CompletionPayment);
                Assert.AreEqual(resultRows[i].MonthlyPayment, expectedResultRows[i].MonthlyPayment);
                Assert.AreEqual(resultRows[i].NumberOfApprentices, expectedResultRows[i].NumberOfApprentices);
                Assert.AreEqual(resultRows[i].NumberOfMonthlyPayments, expectedResultRows[i].NumberOfMonthlyPayments);
                Assert.AreEqual(resultRows[i].StartDate, expectedResultRows[i].StartDate);
                Assert.AreEqual(resultRows[i].TotalCost, expectedResultRows[i].TotalCost);
            }
        }

        public class TestApprenticeship
        {
            public string Apprenticeship { get; set; }
            public string NumberOfApprentices { get; set; }
            public string NumberOfMonths { get; set; }
            public string StartDateMonth { get; set; }
            public string StartDateYear { get; set; }
            public string TotalCost { get; set; }
        }
    }
}
