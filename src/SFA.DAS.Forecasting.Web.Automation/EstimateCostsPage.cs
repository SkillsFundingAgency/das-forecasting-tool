using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class EstimateCostsPage: BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h1[text()='Estimated costs']")] public IWebElement PageHeader { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[text()='Remove']")] public IWebElement RemoveApprenticeshipButton { get; set; }

        public EstimateCostsPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool IsApprenticeshipsTableVisible()
        {
            var aprenticeshipsAddedTabButtonSelector = "//a[text()='Apprenticeships added']";
            PageHeader.WaitForElementIsVisible();
            var tabs = Driver.FindElements(By.XPath(aprenticeshipsAddedTabButtonSelector));
            return tabs.Count > 0;
        }

        public void RemoveFirstApprenticeship()
        {
            RemoveApprenticeshipButton.ClickThisElement();
            var removalPage = new RemoveApprenticeshipPage(Driver);
            removalPage.ConfirmRemoval();
        }
    } 
}