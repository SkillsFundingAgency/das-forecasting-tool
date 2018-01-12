using SFA.DAS.Forecasting.Web.AcceptanceTests.Infrastructure.Registries;
using StructureMap;
using System;
using SFA.DAS.Forecasting.Web.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class BindingBootstrapper: StepsBase
    {
        [BeforeTestRun(Order = 0)]
        public static void SetUpContainer()
        {
            ParentContainer = new Container(new DefaultRegistry());
            if (Config.Environment.Equals("DEV", StringComparison.OrdinalIgnoreCase))
            {
                StartApi("SFA.DAS.Forecasting.Web");
            }
        }

        [AfterTestRun(Order = 999)]
        public static void CleanUpContainer()
        {
            Processes?.ForEach(process => process.Kill());
            ParentContainer.Dispose();
        }

        [BeforeScenario(Order = 0)]
        public void SetUpNestedContainer()
        {
            NestedContainer = ParentContainer.GetNestedContainer();
            WebSite = new ForecastingWebSite(Config.WebSiteUrl, Config.Browser);
        }

        [AfterScenario(Order = 999)]
        public void CleanUpNestedContainer()
        {
            WebSite?.Close();
            NestedContainer?.Dispose();
        }
    }
}