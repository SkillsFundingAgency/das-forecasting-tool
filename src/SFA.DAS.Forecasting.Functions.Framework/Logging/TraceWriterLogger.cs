using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    public class TraceWriterLogger: ILog
    {
        private readonly TraceWriter _traceWriter;
        public TraceWriterLogger(TraceWriter traceWriter)
        {
            _traceWriter = traceWriter ?? throw new ArgumentNullException(nameof(traceWriter));
        }

        public void Trace(string message)
        {
            _traceWriter.Verbose(message);
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            _traceWriter.Verbose(message);
        }

        public void Trace(string message, ILogEntry logEntry)
        {
            _traceWriter.Verbose(message);
        }

        public void Debug(string message)
        {
            _traceWriter.Verbose(message);
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            _traceWriter.Verbose(message);
        }

        public void Debug(string message, ILogEntry logEntry)
        {
            _traceWriter.Verbose(message);
        }

        public void Info(string message)
        {
            _traceWriter.Info(message);
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            _traceWriter.Info(message);
        }

        public void Info(string message, ILogEntry logEntry)
        {
            _traceWriter.Info(message);
        }

        public void Warn(string message)
        {
            _traceWriter.Warning(message);
        }

        public void Warn(string message, IDictionary<string, object> properties)
        {
            _traceWriter.Warning(message);
        }

        public void Warn(string message, ILogEntry logEntry)
        {
            _traceWriter.Warning(message);
        }

        public void Warn(Exception ex, string message)
        {
            _traceWriter.Warning($"{message}. Ex: {ex}");
        }

        public void Warn(Exception ex, string message, IDictionary<string, object> properties)
        {
            _traceWriter.Warning($"{message}. Ex: {ex}");
        }

        public void Warn(Exception ex, string message, ILogEntry logEntry)
        {
            _traceWriter.Warning($"{message}. Ex: {ex}");
        }

        public void Error(Exception ex, string message)
        {
            _traceWriter.Error($"{message}. Ex: {ex}");
        }

        public void Error(Exception ex, string message, IDictionary<string, object> properties)
        {
            _traceWriter.Error($"{message}. Ex: {ex}");
        }

        public void Error(Exception ex, string message, ILogEntry logEntry)
        {
            _traceWriter.Error($"{message}. Ex: {ex}");
        }

        public void Fatal(Exception ex, string message)
        {
            _traceWriter.Error($"Fatal Error!! {message}. Ex: {ex}");
        }

        public void Fatal(Exception ex, string message, IDictionary<string, object> properties)
        {
            _traceWriter.Error($"Fatal Error!! {message}. Ex: {ex}");
        }

        public void Fatal(Exception ex, string message, ILogEntry logEntry)
        {
            _traceWriter.Error($"Fatal Error!! {message}. Ex: {ex}");
        }
    }
}