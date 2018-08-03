using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Telemetry
{
    public class AppInsightsTelemetry : ITelemetry, IDisposable
    {
        private readonly TelemetryClient _telemetry;
        private readonly Dictionary<string, string> _properties;
        public AppInsightsTelemetry(TelemetryClient telemetry)
        {
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            _properties = new Dictionary<string, string>();
        }

        public void AddProperty(string propertyName, string value)
        {
            _properties.Add(propertyName, value);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties)
        {
            _telemetry.TrackEvent(eventName+" Event", _properties.ConcatDictionary(properties));
        }

        public void TrackDuration(string durationName, TimeSpan duration, Dictionary<string, string> properties)
        {
            _telemetry.TrackMetric(durationName+" Duration", duration.TotalMilliseconds, _properties.ConcatDictionary(properties));
        }

        public void TrackDependency(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success,
            Dictionary<string, string> properties)
        {
            _telemetry.TrackDependency(dependencyType, dependencyName+" Dependency", JsonConvert.SerializeObject(_properties.ConcatDictionary(properties)), startTime, duration, success);
        }

        private void ReleaseUnmanagedResources()
        {
            _telemetry?.Flush();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~AppInsightsTelemetry()
        {
            ReleaseUnmanagedResources();
        }
    }
}
