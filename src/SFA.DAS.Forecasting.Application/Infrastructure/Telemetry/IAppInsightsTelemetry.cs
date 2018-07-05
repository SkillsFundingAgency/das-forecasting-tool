using System;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Telemetry
{
	public interface IAppInsightsTelemetry
	{
		void TrackEvent(string functionName, string desc, string methodName);
		void TrackEvent(string functionName, string desc, string methodName, Guid invocationId);
		void TrackException(string functionName, Exception ex, string desc, string methodName);
	}
}