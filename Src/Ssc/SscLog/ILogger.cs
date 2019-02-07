using System.Runtime.CompilerServices;

namespace Ssc.SscLog {
    public interface ILogger {
        LogType LogType { get; }
        LogLevel LogLevel { get; set; }
        string Name { get; }
        bool IsLogging(LogLevel logLevel);

        void Trace(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Trace(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Debug(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Debug(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Info(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Info(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Warn(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Warn(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Error(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Error(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Fatal(object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Fatal(bool condition, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Log(bool condition, LogLevel logLevel, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);

        void Log(LogLevel logLevel, object message,
            [CallerFilePath] string callingFilePath = default,
            [CallerMemberName] string callingMethod = default,
            [CallerLineNumber] int callingFileLineNumber = 0);
    }
}