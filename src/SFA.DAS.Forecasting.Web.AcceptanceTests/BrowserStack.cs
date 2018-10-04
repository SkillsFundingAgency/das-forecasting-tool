using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class BrowserStack
    {
        private BrowserStackDriver bsDriver;
        private string[] tags;

        [BeforeScenario]
        public void BeforeScenario()
        {
            bsDriver = new BrowserStackDriver(ScenarioContext.Current);
            ScenarioContext.Current["bsDriver"] = bsDriver;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            bsDriver.Cleanup();
        }
    }
}