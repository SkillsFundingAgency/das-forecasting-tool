using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Telemetry
{
    public static class TelemetryExtensions
    {
        public static void TrackDependency(this ITelemetry telemetry, DependencyType dependencyType, string dependencyName, DateTimeOffset startTime,
            TimeSpan duration, bool success,
            Dictionary<string, string> properties = null)
        {
            telemetry.TrackDependency(dependencyType.ToString("G"), dependencyName, startTime, duration, success, properties);
        }

        public static void AddEmployerAccountId(this ITelemetry telemetry, long employerAccountId)
        {
            telemetry.AddProperty("Employer Account Id", employerAccountId.ToString());
        }
    }
}