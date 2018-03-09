using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class LoginPage: BasePage
    {
        [FindsBy(How = How.Id, Using = "EmailAddress")] private IWebElement EmailAddress { get; set; }
        [FindsBy(How = How.Id, Using = "Password")] private IWebElement Password { get; set; }
        [FindsBy(How = How.ClassName, Using = "button")] private IWebElement LoginBtn { get; set; }

        public LoginPage(IWebDriver webDriver) : base(webDriver)
        {
        }
        public void LoginAsUser(string userName, string passWord)
        {
            EnterUserName(userName);
            EnterPassWord(passWord);
            ClickLoginButton();
        }

        private void EnterUserName(string userName)
        {
            EmailAddress.EnterTextInThisElement(userName);
        }

        private void EnterPassWord(string passWord)
        {
            Password.EnterTextInThisElement(passWord);
        }

        private void ClickLoginButton()
        {
            LoginBtn.ClickThisElement();
        }
    } 
}