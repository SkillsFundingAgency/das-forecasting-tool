using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class HomePage: BasePage
    {
        [FindsBy(How = How.Id, Using = "service-start")] private IWebElement StartButton;
        [FindsBy(How = How.XPath, Using = "//*[@id=\"content\"]/div/div/form/div[1]/fieldset/label[1]")] private IWebElement UsedServiceBefore { get; set; }
        [FindsBy(How = How.XPath, Using = "//*[@id=\"content\"]/div/div/form/div[1]/fieldset/label[2]")] private IWebElement NotUsedServiceBefore { get; set; }
        [FindsBy(How = How.Id, Using = "submit-button")] private IWebElement Continue { get; set; }

        public HomePage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public LoginPage MoveToLoginPage()
        {
            StartButton.ClickThisElement();
            UsedServiceBefore.CheckThisRadioButton();
            Continue.ClickThisElement();
            return new LoginPage(Driver);
        }
    } 
}