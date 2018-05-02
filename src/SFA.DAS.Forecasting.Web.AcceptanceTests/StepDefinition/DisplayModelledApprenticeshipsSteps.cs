﻿using NUnit.Framework;
using Sfa.Automation.Framework.Extensions;
using SFA.DAS.Forecasting.Web.Automation;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class DisplayModelledApprenticeshipsSteps: StepsBase
    {
        [Given(@"that I have added the following apprenticeships")]
        public void GivenThatIHaveAddedTheFollowingApprenticeships(Table table)
        {
            var apprenticeships = table.CreateSet<TestApprenticeship>().ToList();
            Set(apprenticeships);

            var estimateCostsPage = WebSite.NavigateToEstimageCostsPage();
            var isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
            while (isAnyapprenticeshipExist)
            {
                estimateCostsPage.RemoveFirstApprenticeship();
                isAnyapprenticeshipExist = estimateCostsPage.IsApprenticeshipsTableVisible();
            }
            estimateCostsPage = WebSite.NavigateToEstimageCostsPage();
            foreach(var apprenticeship in apprenticeships)
            {
                var addApprenticeshipPage = estimateCostsPage.AddApprenticeships();
                addApprenticeshipPage.SelectApprenticeshipDropdown.SelectDropDown(WebSite.getDriver(), apprenticeship.Apprenticeship);
                addApprenticeshipPage.PageHeader.ClickThisElement();
                addApprenticeshipPage.NumberOfApprenticesInput.EnterTextInThisElement(apprenticeship.NumberOfApprentices);
                addApprenticeshipPage.NumberOfMonthsInput.EnterTextInThisElement(apprenticeship.NumberOfMonths);
                addApprenticeshipPage.StartDateMonthInput.EnterTextInThisElement(apprenticeship.StartDateMonth);
                addApprenticeshipPage.StartDateYearInput.EnterTextInThisElement(apprenticeship.StartDateYear);
                addApprenticeshipPage.ContinueButton.ClickThisElement();
            }
        }
        
        [When(@"the modelled apprenticeships page is displayed")]
        public void WhenTheModelledApprenticeshipsPageIsDisplayed()
        {
            var estimateCostsPage = WebSite.NavigateToEstimageCostsPage();
            Set(estimateCostsPage);
            Assert.IsTrue(estimateCostsPage.PageHeader.Displayed);
        }
        
        [Then(@"the column headings are displayed")]
        public void ThenTheColumnHeadingsAreDisplayed()
        {
            var estimateCostsPage = Get<EstimateCostsPage>();
            var tableHeaders = estimateCostsPage.GetApprenticeshipsAddedTableHeaders();
            var expectedHeaders = new String[] {
                "Apprenticeship\r\nLevel",
                "Number of apprentices",
                "Start date",
                "Total Cost",
                "Number of monthly payments",
                "Monthly payment",
                "Completion payment"
            };
            Assert.AreEqual(expectedHeaders, tableHeaders);

        }

        [Then(@"each added apprenticeship is displayed in a separate row")]
        public void ThenEachAddedApprenticeshipIsDisplayedInASeparateRow()
        {
            var estimateCostsPage = Get<EstimateCostsPage>();
            var resultRows = estimateCostsPage.GetApprenticeshipsTableContent();
            Assert.AreEqual(3, resultRows.Length);
        }
        
        [Then(@"the apprenticeship with the earliest start date is shown first")]
        public void ThenTheApprenticeshipWithTheEarliestStartDateIsShownFirst()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the other apprenticeships are in order of start date")]
        public void ThenTheOtherApprenticeshipsAreInOrderOfStartDate()
        {
            //ScenarioContext.Current.Pending();
        }
        
        [Then(@"the details against each apprenticeship match what was entered")]
        public void ThenTheDetailsAgainstEachApprenticeshipMatchWhatWasEntered()
        {
            //ScenarioContext.Current.Pending();
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