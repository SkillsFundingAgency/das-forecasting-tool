using System;
using System.Linq;
using SFA.DAS.Forecasting.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Projections.Steps
{
    [Binding]
    public class PublishProjectionCreatedEvent_CI_963Steps : BindingBootstrapper
    {
        [Scope(Feature = "Publish Projection Created Event - CI-963")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Projections.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Then(@"the Account Projection Event is published")]
        public void ThenTheAccountProjectionEventIsPublished()
        {
            WaitForIt(() =>
                {
                    return AccountProjectionCreatedEventHandler.ReceivedEvents.Any(w => w.EmployerAccountId == 12345);
                },"Failed to find Published Account Projection event message");
        }
    }
}
