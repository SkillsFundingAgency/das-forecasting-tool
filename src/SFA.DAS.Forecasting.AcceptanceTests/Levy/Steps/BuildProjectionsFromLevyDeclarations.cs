using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy.Steps
{
    [Binding]
    public class BuildProjectionsFromLevyDeclarations : StepsBase
    {
        [Scope(Feature = "Build Projections From Levy Declarations")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Levy.Functions");
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
        }
        
        [Given(@"no account projections have been generated")]
        public void GivenNoAccountProjectionsHaveBeenGenerated()
        {
            DeleteAccountProjections();
        }
    }
}