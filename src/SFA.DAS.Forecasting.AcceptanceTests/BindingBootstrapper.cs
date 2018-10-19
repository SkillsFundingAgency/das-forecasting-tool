using System.Threading;
using NServiceBus;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    [Binding]
    public class BindingBootstrapper : StepsBase
    {
        public static EndpointConfiguration EndpointConfiguration { get; private set; }
            
        [BeforeTestRun(Order = 0)]
        public static void SetUpContainer()
        {
            ParentContainer = new Container(new DefaultRegistry());

            EndpointConfiguration.UseAzureServiceBusTransport(Config.EventsNServiceBusUseDevTransport,
                    () => Config.NServiceBusConnectionString, r => { })
                .UseErrorQueue()
                .UseInstallers()
                .UseNewtonsoftJsonSerializer()
                .UseStructureMapBuilder(ParentContainer)
                .PurgeOnStartup(true);
        }
        [BeforeTestRun(Order = 99)]
        public static void StartBus()
        {
            MessageSession = Endpoint.Start(EndpointConfiguration).Result;
            ParentContainer.Configure(c =>
            {
                c.For<IMessageSession>().Use(MessageSession);
            });
        }

        [BeforeScenario(Order = 0)]
        public void SetupNestedContainer()
        {
            NestedContainer = ParentContainer.GetNestedContainer();
            CommitmentType = CommitmentType.LevyFunded;
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
                //process.WaitForExit()
                var processName = process.ProcessName;
                if (!process.HasExited)
                    process.Kill();
                //foreach (var process1 in Process.GetProcessesByName(processName))
                //{
                //    process1.Kill();
                //}
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
