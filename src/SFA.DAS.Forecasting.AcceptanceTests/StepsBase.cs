using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    [Binding]
    public class StepsBase
    {
        protected static IContainer ParentContainer { get; set; }

        protected static Config Config => ParentContainer.GetInstance<Config>();
    }
}
