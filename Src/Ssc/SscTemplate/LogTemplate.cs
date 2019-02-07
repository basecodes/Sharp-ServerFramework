using Ssc.SscLog;

namespace Ssc.SscTemplate {
    public class LogTemplate<T> {
        private static readonly Logger Logger = LogManager.GetLogger<T>(LogType.High);
    }
}