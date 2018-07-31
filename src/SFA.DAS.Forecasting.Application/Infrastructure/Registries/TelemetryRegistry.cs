using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
	public class TelemetryRegistry : Registry
	{
		public TelemetryRegistry()
		{

		    ForSingletonOf<TelemetryConfiguration>().Use<TelemetryConfiguration>()
		        .Ctor<string>(ConfigurationHelper.GetAppSetting("APPINSIGHTS_INSTRUMENTATIONKEY", false));
		    ForSingletonOf<TelemetryClient>().Use<TelemetryClient>().Ctor<TelemetryConfiguration>().Is(ctx => ctx.GetInstance<TelemetryConfiguration>());
//			For<IAppInsightsTelemetry>().Use<AppInsightsTelemetry>();
		}
	}
}
