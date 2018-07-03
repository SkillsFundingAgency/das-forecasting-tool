using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Telemetry
{
	public class AppInsightsTelemetry : IAppInsightsTelemetry
	{
		private readonly IApplicationConfiguration _configuration;
		private TelemetryClient _telemetry;

		public AppInsightsTelemetry(IApplicationConfiguration configuration)
		{
			_configuration = configuration;
			_telemetry = new TelemetryClient();
			TelemetryConfiguration.Active.InstrumentationKey = _configuration.AppInsightsInstrumentationKey;
		}

		public void TrackEvent(string functionName, string desc, string methodName)
		{
			_telemetry.TrackEvent($"{functionName} - {methodName}: {desc}");
		}

		public void TrackException(string functionName, Exception ex, string desc, string methodName)
		{
			var properties = new Dictionary<string, string>() { { "Function", functionName }, { "Method", methodName }, { "Description", desc } };
			_telemetry.TrackException(ex, properties);
		}
	}
}
