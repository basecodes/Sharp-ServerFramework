using System;
using Ssc.SscLog;

namespace Ssc.SscConfiguration {

    [Flags]
    public enum OutputType {
        File = 0x1,
        Console = 0x2
    }

    [Serializable]
    public class LogConfig {
        public LogLevel FilterLogLevel = LogLevel.All;
        public LogLevel ForceLogLevel = LogLevel.All;
        public LogLevel GlobalLogLevel = LogLevel.All;
        public LogSwitch LogSwitch = LogSwitch.On;
        public LogType LogType = LogType.High;
        public OutputType OutputType = OutputType.Console;
        // 文件名包含路径
        public string FileName = "./Log/Log.txt";
        // 写入文件的持续时间，单位秒
        public int WriteInterval = 3;

        public override string ToString() {
            var str = "";
            var fields = GetType().GetFields();
            foreach (var item in fields) {
                var value = item.GetValue(this);
                str += item.Name+" : " + value + "\n";
            }

            return str;
        }
    }
}