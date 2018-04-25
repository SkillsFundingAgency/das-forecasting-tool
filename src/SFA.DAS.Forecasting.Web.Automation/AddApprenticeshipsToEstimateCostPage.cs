﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class AddApprenticeshipsToEstimateCostPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h1[text()='Add apprenticeships to estimate cost']")] public IWebElement PageHeader { get; set; }
        [FindsBy(How = How.Id, Using = "chosse-apprenticeship")] public IWebElement SelectApprenticeshipDropdown { get; set; }
        [FindsBy(How = How.Id, Using = "no-of-app")] public IWebElement NumberOfApprenticesInput { get; set; }
        [FindsBy(How = How.Id, Using = "levy-length")] public IWebElement NumberOfMonthsInput { get; set; }
        [FindsBy(How = How.Id, Using = "startDateMonth")] public IWebElement StartDateMonthInput { get; set; }
        [FindsBy(How = How.Id, Using = "startDateYear")] public IWebElement StartDateYearInput { get; set; }
        [FindsBy(How = How.Id, Using = "levy-value")] public IWebElement TotalCostInput { get; set; }
        [FindsBy(How = How.Id, Using = "save")] public IWebElement ContinueButton { get; set; }
        

        public AddApprenticeshipsToEstimateCostPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool IsPageVisible()
        {
            return PageHeader.Displayed;
        }
    } 
}