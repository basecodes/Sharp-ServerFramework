using NUnit.Framework;
using Ssc.SscLog;

namespace Test {
    [TestFixture]
    public class TestLog {

        private static readonly Logger Logger = LogManager.GetLogger<TestLog>(LogType.High);

        [Test]
        public void Test() {
            Logger.Trace("Trace");
            Logger.Debug("Debug");
            Logger.Info("Info");
            Logger.Error("Error");
            Logger.Fatal("Fatal");
        }
    }
}