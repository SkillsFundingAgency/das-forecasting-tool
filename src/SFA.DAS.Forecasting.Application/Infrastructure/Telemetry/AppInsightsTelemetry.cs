using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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

		public void Debug(string functionName, string desc, string methodName)
		{
			TelemetryTrackEvent("Debug", functionName, desc, methodName);
		}

		public void Debug(string functionName, string desc, string methodName, Guid operationId)
		{
			TelemetryTrackEvent("Debug", functionName, desc, methodName, operationId);
		}

		public void Info(string functionName, string desc, string methodName)
		{
			TelemetryTrackEvent("Info", functionName, desc, methodName);
		}

		public void Info(string functionName, string desc, string methodName, Guid operationId)
		{
			TelemetryTrackEvent("Info", functionName, desc, methodName, operationId);
		}

		public void Warning(string functionName, string desc, string methodName)
		{
			TelemetryTrackEvent("Warning", functionName, desc, methodName);		
		}

		public void Warning(string functionName, string desc, string methodName, Guid operationId)
		{
			TelemetryTrackEvent("Warning", functionName, desc, methodName, operationId);
		}

		public void Error(string functionName, Exception ex, string desc, string methodName)
		{
			TelemetryTrackException("Error", functionName, ex, desc, methodName);
		}

		public void Error(string functionName, Exception ex, string desc, string methodName, Guid operationId)
		{
			TelemetryTrackException("Error", functionName, ex, desc, methodName, operationId);
		}

		private void TelemetryTrackEvent(string logLevel, string functionName, string desc, string methodName)
		{
			_telemetry.TrackEvent($"{logLevel.ToUpper()} --> {functionName} - {methodName}: {desc}");
		}

		private void TelemetryTrackEvent(string logLevel, string functionName, string desc, string methodName, Guid operationId)
		{
			_telemetry.Context.Operation.Id = operationId.ToString();
			TelemetryTrackEvent(logLevel, functionName, desc, methodName);
		}

		private void TelemetryTrackException(string logLevel, string functionName, Exception ex, string desc, string methodName)
		{
			var properties = new Dictionary<string, string>() { { "Function", functionName }, { "Method", methodName }, { "Description", desc } };
			_telemetry.TrackException(ex, properties);
		}

		private void TelemetryTrackException(string logLevel, string functionName, Exception ex, string desc, string methodName, Guid operationId)
		{
			_telemetry.Context.Operation.Id = operationId.ToString();
			TelemetryTrackException("Error", functionName, ex, desc, methodName);
		}
	}
}
