using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class RemoveApprenticeshipPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//*[text()='Yes, I do want to remove this apprenticeship']")] public IWebElement ConfirmCheckbox { get; set; }
        [FindsBy(How = How.CssSelector, Using = "button[type='submit']")] public IWebElement ContinueButton { get; set; }

        [FindsBy(How = How.Id, Using = "radio-2"), CacheLookup]
        private IWebElement _noRadioButton;


        public RemoveApprenticeshipPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool IsNoRadioButtonSelected => _noRadioButton.Selected;

        public void ClickContinueButton()
        {
            ContinueButton.Click();
        }

        public void ConfirmRemoval()
        {
            ConfirmCheckbox.CheckThisRadioButton();
            ContinueButton.ClickThisElement();
        }

        public bool IsPageLoaded => ConfirmCheckbox.Displayed;
    } 
}