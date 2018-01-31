using System;
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
