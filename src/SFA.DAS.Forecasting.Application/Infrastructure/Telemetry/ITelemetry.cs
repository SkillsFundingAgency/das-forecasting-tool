using System;
using System.Collections.Generic;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Telemetry
{
    public interface ITelemetry
    {
        void AddProperty(string propertyName, string value);
        void TrackEvent(string eventName, Dictionary<string, string> properties = null);
        void TrackDuration(string durationName, TimeSpan duration, Dictionary<string, string> properties = null);
        void TrackDependency(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success, Dictionary<string, string> properties = null);
    }
}