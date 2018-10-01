using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Sfa.Automation.Framework.Selenium;

namespace SFA.DAS.Forecasting.Web.Automation.Estimation
{
    public class EditApprenticeshipsPage : AddEditApprenticeshipPage
    {
        [FindsBy(How = How.CssSelector, Using = "button[type='submit']")] public IWebElement ContinueButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".form-group>label:nth-child(2)"), CacheLookup]
        private IWebElement _apprentishipName;

        [FindsBy(How = How.Id, Using = "no-of-app"), CacheLookup]
        private IWebElement _numberOfApprentice;

        [FindsBy(How = How.Id, Using = "apprenticeship-length"), CacheLookup]
        private IWebElement _apprenticeshipLength;

        [FindsBy(How = How.Id, Using = "startDateMonth"), CacheLookup]
        private IWebElement _startDateMonth;

        [FindsBy(How = How.Id, Using = "startDateYear"), CacheLookup]
        private IWebElement _startDateYear;

        [FindsBy(How = How.Id, Using = "total-funding-cost"), CacheLookup]
        private IWebElement _totalFundingCost;

        [FindsBy(How = How.ClassName, Using = "error-summary"), CacheLookup]
        private IWebElement _errorSummary;

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

        [FindsBy(How = How.CssSelector, Using = "h1"), CacheLookup]
        private IWebElement _pageHeader;

        [FindsBy(How = How.Id, Using = "total-cap-details"), CacheLookup]
        private IWebElement _totalCapDetails;

        [FindsBy(How = How.CssSelector, Using = "a[href *= '/accounts/XVJWDM/forecasting/estimations/default']")]
        private IWebElement _cancelButton;

        [FindsBy(How = How.ClassName, Using = "link-back"), CacheLookup]
        private IWebElement _backLink;

        public EditApprenticeshipsPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public void ClickOnBackLink()
        {
            _backLink.Click();
        }

        public void ClickOnCancelButton()
        {
            _cancelButton.Click();
        }

        public void ClickOnPageHeader()
        {
            _pageHeader.Click();
        }

        public string TotalFundingCapDetails => _totalCapDetails.Text;

        public bool IsPageLoaded => ContinueButton.Displayed && Driver.Url.Contains("EditApprenticeships");

        public string ApprentishipName => _apprentishipName.Text;

        public string NumberOfApprenticeships => _numberOfApprentice.GetAttribute("value");

        public string ApprenticeshipLength => _apprenticeshipLength.GetAttribute("value");

        public string StartDateMonth => _startDateMonth.GetAttribute("value");

        public string StartDateYear => _startDateYear.GetAttribute("value");

        public string TotalFundingCost => _totalFundingCost.GetAttribute("value");

        public void ChangeStartDateMonth(string value)
        {
            _startDateMonth.Clear();
            _startDateMonth.EnterTextInThisElement(value);
        }

        public void ChangeStartDateYear(string value)
        {
            _startDateYear.Clear();
            _startDateYear.EnterTextInThisElement(value);
        }

        public void ChangeTheFunding(string value)
        {
            _totalFundingCost.Clear();
            _totalFundingCost.EnterTextInThisElement(value);
        }

        public void ChangeTheNumberOfApprentice(string value)
        {
            _numberOfApprentice.Clear();
            _numberOfApprentice.EnterTextInThisElement(value);
        }

        public bool IsErrorSummaryDisplayed => _errorSummary.Displayed;

        public bool IsTheStartDateCannotBeInThePastErrorDisplayed => _theStartDateCannotBeInThePastError.Displayed;
    }
}
