using Common.CSharp;
using Ssc.Ssc;
using Ssc.SscLog;
using System;
using System.Linq;

namespace CSharpModule.Controllers {

    public class TestController : Controller {

        private static readonly Logger Logger = LogManager.GetLogger<TestController>(LogType.High);

        public TestController() {
            Initialize();
        }

        private void Initialize() {
            Register<ITestController>(
                (tc) => tc.Test(0, null, null, null),
                () => this.Test(0, null, null, null));

            Register<ITestController>(
                (tc) => tc.Test(0, null, null, null, null),
                () => this.Test(0, null, null, null, null));

            Register<ITestController>(
                (tc) => tc.Test(0, null, null, null, null, null),
                () => this.Test(0, null, null, null, null, null));
        }

        public bool Test(int num, string str, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str);
            var value = num + " " + str;
            Invoke<ITestRequest>(() => (req) => req.TestRequest(value), peer);
            return true;
        }

        public bool Test(int num, string str, int[] array, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str); Logger.Info(num);
            Logger.Info(str);

            foreach (var item in array) {
                Logger.Info(item);
            }
            return true;
        }

        public bool Test(int num, string str, int[] array, ITestPacket[] testPackets, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str); Logger.Info(num);
            Logger.Info(str);

            foreach (var item in array) {
                Logger.Info(item);
            }

            foreach (var item in testPackets) {
                Logger.Info(item);
            }

            var value = num + " " + str;
            var packet = testPackets.First();
            Invoke<ITestRequest>(() => (req) => req.TestRequest(value, packet), peer);
            return true;
        }
    }
}