using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ssc.SscConfiguration;

namespace Ssc.SscLog {
    public class LogManager {
        public static bool EnableCurrentClassLogger = true;

        private static readonly Dictionary<string, Logger> _loggers;
        private static LogHandler _logHandler;
        private static bool _isInitialized;
        public static LogConfig LogConfig { get; }

        static LogManager() {
            LogConfig = new LogConfig();
            _loggers = new Dictionary<string, Logger>();
        }

        public static void Initialize(LogHandler appender) {
            _isInitialized = true;
            AddAppender(appender);
        }

        public static void AddAppender(LogHandler appender) {
            _logHandler += appender;
            foreach (var logger in _loggers.Values) logger.OnLog += appender;
        }

        public static void RemoveAppender(LogHandler appender) {
            _logHandler -= appender;
            foreach (var logger in _loggers.Values) logger.OnLog -= appender;
        }

        private static Logger CreateLogger<T>(LogType logType) {
            return CreateLogger(typeof(T), logType);
        }

        private static Logger CreateLogger(Type classType, LogType logType) {
            var logger = new Logger(classType, logType) {
                LogLevel = LogConfig.GlobalLogLevel
            };
            return logger;
        }

        public static Logger GetLogger<T>(LogType logType) {
            return GetLogger(typeof(T), logType);
        }

        public static Logger GetLogger(Type type, LogType logType) {
            var name = type.Name;
            if (!_loggers.TryGetValue(name, out var logger)) {
                logger = CreateLogger(type, logType);
                _loggers.Add(name, logger);
                if (_isInitialized) logger.OnLog += _logHandler;
            }

            return logger;
        }

        public static Logger GetCurrentClassLogger(LogType logType) {
            if (EnableCurrentClassLogger) {
                var frame = new StackFrame(1, false);
                return GetLogger(frame.GetMethod().DeclaringType, logType);
            }

            return Logs.Logger;
        }

        public static void Reset() {
            _loggers.Clear();
        }
    }
}