﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.NLog.Logger;

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
            _telemetry.TrackEvent(eventName, _properties.ConcatDictionary(properties));
        }

        public void TrackDuration(string durationName, TimeSpan duration, Dictionary<string, string> properties)
        {
            _telemetry.TrackMetric(durationName, duration.TotalMilliseconds, _properties.ConcatDictionary(properties));
        }

        public void TrackDependency(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success,
            Dictionary<string, string> properties)
        {
            _telemetry.TrackDependency(dependencyType, dependencyName, JsonConvert.SerializeObject(_properties.ConcatDictionary(properties)), startTime, duration, success);
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
