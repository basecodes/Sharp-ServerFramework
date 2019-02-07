using System;
using System.Runtime.CompilerServices;

namespace Ssc.SscLog {
    public class Logger : ILogger {
        public Logger(Type type, LogType logType) {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Name = type.Name;
            LogType = logType;
            LogLevel = LogLevel.All;
        }

        public LogType LogType { get; }
        public LogLevel LogLevel { get; set; }
        public string Name { get; }

        internal event LogHandler OnLog;

        public bool IsLogging(LogLevel logLevel) {
            return (LogType == LogManager.LogConfig.LogType || LogManager.LogConfig.LogType == LogType.Global) &&
                   (LogLevel <= logLevel || logLevel >= LogManager.LogConfig.GlobalLogLevel);
        }

        public void Trace(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            Log(LogLevel.Trace, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Trace(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(LogLevel.Trace, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Debug(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            Log(LogLevel.Debug, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Debug(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(LogLevel.Debug, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Info(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            Log(LogLevel.Info, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Info(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(LogLevel.Info, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Warn(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            Log(LogLevel.Warn, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Warn(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(LogLevel.Warn, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Error(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            Log(LogLevel.Error, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Error(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(LogLevel.Error, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Fatal(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            Log(LogLevel.Fatal, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Fatal(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(LogLevel.Fatal, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Log(bool condition, LogLevel logLevel, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            if (condition) Log(logLevel, message, callingFilePath, callingMethod, callingFileLineNumber);
        }

        public void Log(LogLevel logLevel, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0) {
            OnLog?.Invoke(this, logLevel, message, callingFilePath, callingMethod, callingFileLineNumber);
        }
    }
}