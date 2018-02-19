using Nancy;
using Nancy.Configuration;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class NancyBootstrapper: DefaultNancyBootstrapper
    {
        public override void Configure(INancyEnvironment environment)
        {
            var config = new Nancy.TraceConfiguration(enabled: false, displayErrorTraces: true);
            environment.AddValue(config);
        }
    }
}