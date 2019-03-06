using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Extensions;
using Sfa.Automation.Framework.Selenium;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Web.Automation
{
    public class AddApprenticeshipsToEstimateCostPage : AddEditApprenticeshipPage
    {
        [FindsBy(How = How.XPath, Using = "//h1[text()='Add apprenticeships to estimate cost']")] public IWebElement PageHeader { get; set; }
        [FindsBy(How = How.Id, Using = "choose-apprenticeship")] public IWebElement SelectApprenticeshipDropdown { get; set; }
        [FindsBy(How = How.Id, Using = "save")] public IWebElement ContinueButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "error-summary")]
        private IWebElement _errorBox;
        
        [FindsBy(How = How.CssSelector, Using = "a[href *= '#error-message-Course']")]
        private IWebElement _youMustChooseOneApprenticeshipError;

        [FindsBy(How = How.CssSelector, Using = "a[href *= '#error-message-NumberOfApprentices']")]
        private IWebElement _makeSureYouHaveAtLeastOneOrMoreApprenticesError;

        [FindsBy(How = How.CssSelector, Using = "a[href *= '#error-message-StartDateMonth']")]
        private IWebElement _theStartMonthWasNotEnteredError;

        [FindsBy(How = How.CssSelector, Using = "a[href *= '#error-message-StartDateYear']")]
        private IWebElement _theStartYearWasNotEnteredError;

        [FindsBy(How = How.CssSelector, Using = "a[href *= '#error-message-StartDate']")]
        private IWebElement _theStartDateCannotBeInThePastError;

        [FindsBy(How = How.CssSelector, Using = "a[href *= '#error-message-TotalCostAsString']")]
        private IWebElement _youMustEnterAnumberThatIsAboveZeroError;

        public AddApprenticeshipsToEstimateCostPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool IsErrorBoxDisplayed => _errorBox.Displayed;

        public bool IsYouMustChooseOneApprenticeshipErrorDisplayed => _youMustChooseOneApprenticeshipError.Displayed;

        public bool IsMakeSureYouHaveAtLeastOneOrMoreApprenticesErrorDisplayed => _makeSureYouHaveAtLeastOneOrMoreApprenticesError.Displayed;

        public bool IsTheStartMonthWasNotEnteredErrorDisplayed => _theStartMonthWasNotEnteredError.Displayed;

        public bool IsTheStartYearWasNotEnteredErrorDisplayed => _theStartYearWasNotEnteredError.Displayed;

        public bool IsYouMustEnterAnumberThatIsAboveZeroErrorDisplayed => _youMustEnterAnumberThatIsAboveZeroError.Displayed;

        public bool IsTheStartDateCannotBeInThePastErrorDisplayed => _theStartDateCannotBeInThePastError.Displayed;

        public bool IsPageVisible()
        {
            return PageHeader.Displayed;
        }
    }

    public class AddEditApprenticeshipPage : BasePage
    {
        [FindsBy(How = How.CssSelector, Using = ".heading-xlarge")] public IWebElement Heading { get; set; }

        [FindsBy(How = How.Id, Using = "IsTransferFunded")] public IWebElement UseTransferAllowance { get; set; }
        [FindsBy(How = How.Id, Using = "no-of-app")] public IWebElement NumberOfApprenticesInput { get; set; }
        [FindsBy(How = How.Id, Using = "apprenticeship-length")] public IWebElement NumberOfMonthsInput { get; set; }
        [FindsBy(How = How.Id, Using = "startDateMonth")] public IWebElement StartDateMonthInput { get; set; }
        [FindsBy(How = How.Id, Using = "startDateYear")] public IWebElement StartDateYearInput { get; set; }
        [FindsBy(How = How.Id, Using = "total-funding-cost")] public IWebElement TotalCostInput { get; set; }

        protected AddEditApprenticeshipPage(IWebDriver webDriver) : base(webDriver)
        {
        }
    }

}