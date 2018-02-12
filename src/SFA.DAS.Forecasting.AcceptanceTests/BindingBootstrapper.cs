using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    [Binding]
    public class BindingBootstrapper : StepsBase
    {
        [BeforeTestRun(Order = 0)]
        public static void SetUpContainer()
        {
            ParentContainer = new Container(new DefaultRegistry());
        }

        [BeforeScenario(Order = 0)]
        public void SetupNestedContainer()
        {
            NestedContainer = ParentContainer.GetNestedContainer();
        }

        [AfterScenario(Order = 99)]
        public void CleanupNestedContainer()
        {
            NestedContainer?.Dispose();
            NestedContainer = null;
        }

        [AfterFeature(Order = 0)]
        public static void CleanUpFunctionProcesses()
        {
            Processes?.ForEach(process => process.Kill());
        }



        [AfterTestRun(Order = 999)]
        public static void CleanUpContainer()
        {
            ParentContainer.Dispose();
        }
    }
}
