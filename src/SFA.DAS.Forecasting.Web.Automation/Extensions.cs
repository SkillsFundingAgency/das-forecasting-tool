using System;
using System.IO;
using OpenQA.Selenium;
using Sfa.Automation.Framework.Extensions;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public static class Extensions
    {
        public static void ClickThisElement(this IWebElement element)
        {
            element.WaitForElementIsVisible();
            element.Click(Sfa.Automation.Framework.Selenium.BasePage.Driver);
        }

        public static void EnterTextInThisElement(this IWebElement element, string textToEnter)
        {
            element.WaitForElementIsVisible();
            element.EnterText(Sfa.Automation.Framework.Selenium.BasePage.Driver, textToEnter);
        }

        public static void CheckThisRadioButton(this IWebElement element)
        {
            element.WaitForElementIsVisible();
            element.SelectCheckBox(Sfa.Automation.Framework.Selenium.BasePage.Driver);
        }

        public static string GetMessage(this IWebElement element)
        {
            element.WaitForElementIsVisible();
            return element.GetTextWhenShown();
        }

        public static Uri Combine(this Uri uri, string part)
        {
            return new Uri(Path.Combine(uri.ToString(), part).Replace("\\", "/"));
        }
    }
}