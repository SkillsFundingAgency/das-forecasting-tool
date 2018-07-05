﻿using System;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Telemetry
{
	public interface IAppInsightsTelemetry
	{
		void Debug(string functionName, string desc, string methodName);
		void Debug(string functionName, string desc, string methodName, Guid operationId);
		void Info(string functionName, string desc, string methodName);
		void Info(string functionName, string desc, string methodName, Guid operationId);
		void Warning(string functionName, string desc, string methodName);
		void Warning(string functionName, string desc, string methodName, Guid operationId);
		void Error(string functionName, Exception ex, string desc, string methodName);
		void Error(string functionName, Exception ex, string desc, string methodName, Guid operationId);
	}
}