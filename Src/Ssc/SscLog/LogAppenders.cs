namespace Ssc.SscLog {
    public class LogAppenders {
        public virtual bool ConsoleAppender(Logger logger, LogLevel logLevel, object message, string callingFilePath,
            string callingMethod, int callingFileLineNumber) {
            if (LogManager.LogConfig.LogSwitch == LogSwitch.Off) return false;

            if (logger.LogType < LogManager.LogConfig.LogType && LogManager.LogConfig.LogType != LogType.Global)
                return false;

            if (LogManager.LogConfig.FilterLogLevel != logLevel &&
                LogManager.LogConfig.FilterLogLevel != LogLevel.All) return false;

            if (logLevel < LogManager.LogConfig.ForceLogLevel) return false;

            if (logger.LogLevel > logLevel || logger.LogLevel < LogManager.LogConfig.GlobalLogLevel) return false;

            return true;
        }
    }
}