using System;
using System.Threading;
using Nancy;
using Nancy.Hosting.Self;
using SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    [Binding]
    public class BindingBootstrapper : StepsBase
    {
        private static NancyHost Host { get; set; }
        [BeforeTestRun(Order = 0)]
        public static void SetUpContainer()
        {
            ParentContainer = new Container(new DefaultRegistry());
            if (Config.IsDevEnvironment)
            {
                var config = new HostConfiguration
                {
                    
                    UrlReservations = new UrlReservations { CreateAutomatically = true, User = "Everyone" }
                };
                Host = new NancyHost(config, new Uri("http://localhost:50002/"));
                Host.Start();
            }
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
            Host?.Stop();
            Host?.Dispose();
            Thread.Sleep(500);
            ParentContainer.Dispose();
        }
    }
}
