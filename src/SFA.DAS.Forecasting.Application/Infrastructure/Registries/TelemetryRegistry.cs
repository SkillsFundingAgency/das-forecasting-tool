using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
	public class TelemetryRegistry : Registry
	{
		public TelemetryRegistry()
		{
			For<IAppInsightsTelemetry>().Use<AppInsightsTelemetry>();
		}
	}
}
