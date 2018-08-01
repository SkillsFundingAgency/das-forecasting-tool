using System;
using System.Collections.Generic;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Functions.Framework.Logging
{
    public class CompositeLogger : ILog
    {
        private readonly NLogLogger _nLogLogger;
        private readonly TraceWriterLogger _traceWriterLogger;

        public CompositeLogger(NLogLogger nLogLogger, TraceWriterLogger traceWriterLogger)
        {
            _nLogLogger = nLogLogger ?? throw new ArgumentNullException(nameof(nLogLogger));
            _traceWriterLogger = traceWriterLogger ?? throw new ArgumentNullException(nameof(traceWriterLogger));
        }

        public void Trace(string message)
        {
            _nLogLogger.Trace(message);
            _traceWriterLogger.Trace(message);
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Trace(message, properties);
            _traceWriterLogger.Trace(message, properties);
        }

        public void Trace(string message, ILogEntry logEntry)
        {
            _nLogLogger.Trace(message, logEntry);
            _traceWriterLogger.Trace(message, logEntry);
        }

        public void Debug(string message)
        {
            _nLogLogger.Debug(message);
            _traceWriterLogger.Debug(message);
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Debug(message, properties);
            _traceWriterLogger.Debug(message, properties);
        }

        public void Debug(string message, ILogEntry logEntry)
        {
            _nLogLogger.Debug(message, logEntry);
            _traceWriterLogger.Debug(message, logEntry);
        }

        public void Info(string message)
        {
            _nLogLogger.Info(message);
            _traceWriterLogger.Info(message);
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Info(message, properties);
            _traceWriterLogger.Info(message, properties);
        }

        public void Info(string message, ILogEntry logEntry)
        {
            _nLogLogger.Info(message, logEntry);
            _traceWriterLogger.Info(message, logEntry);
        }

        public void Warn(string message)
        {
            _nLogLogger.Warn(message);
            _traceWriterLogger.Warn(message);
        }

        public void Warn(string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Warn(message, properties);
            _traceWriterLogger.Warn(message, properties);
        }

        public void Warn(string message, ILogEntry logEntry)
        {
            _nLogLogger.Warn(message, logEntry);
            _traceWriterLogger.Warn(message, logEntry);
        }

        public void Warn(Exception ex, string message)
        {
            _nLogLogger.Warn(ex, message);
            _traceWriterLogger.Warn(ex, message);
        }

        public void Warn(Exception ex, string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Warn(ex, message, properties);
            _traceWriterLogger.Warn(ex, message, properties);
        }

        public void Warn(Exception ex, string message, ILogEntry logEntry)
        {
            _nLogLogger.Warn(ex, message, logEntry);
            _traceWriterLogger.Warn(ex, message, logEntry);
        }

        public void Error(Exception ex, string message)
        {
            _nLogLogger.Error(ex, message);
            _traceWriterLogger.Error(ex, message);
        }

        public void Error(Exception ex, string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Error(ex, message, properties);
            _traceWriterLogger.Error(ex, message, properties);
        }

        public void Error(Exception ex, string message, ILogEntry logEntry)
        {
            _nLogLogger.Error(ex, message, logEntry);
            _traceWriterLogger.Error(ex, message, logEntry);
        }

        public void Fatal(Exception ex, string message)
        {
            _nLogLogger.Fatal(ex, message);
            _traceWriterLogger.Fatal(ex, message);
        }

        public void Fatal(Exception ex, string message, IDictionary<string, object> properties)
        {
            _nLogLogger.Fatal(ex, message, properties);
            _traceWriterLogger.Fatal(ex, message, properties);
        }

        public void Fatal(Exception ex, string message, ILogEntry logEntry)
        {
            _nLogLogger.Fatal(ex, message, logEntry);
            _traceWriterLogger.Fatal(ex, message, logEntry);
        }
    }
}