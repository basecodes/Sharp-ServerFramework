namespace Ssc.SscLog {
    public enum LogType : byte {
        Global,
        Low,
        Middle,
        High
    }

    public enum LogSwitch : byte {
        Off,
        On
    }

    public enum LogLevel : byte {
        All,
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}