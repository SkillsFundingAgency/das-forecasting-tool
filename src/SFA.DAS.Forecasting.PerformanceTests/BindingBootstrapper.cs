using System.Threading;
using SFA.DAS.Forecasting.PerformanceTests.Infrastructure.Registries;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.PerformanceTests
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
            Processes?.ForEach(process =>
            {
                var processName = process.ProcessName;
                if (!process.HasExited)
                    process.Kill();

            });
            Processes?.Clear();
        }

        [AfterTestRun(Order = 999)]
        public static void CleanUpContainer()
        {
            Thread.Sleep(500);
            ParentContainer.Dispose();
        }
    }
}
