using Common.CSharp;
using Ssc;
using Ssc.Ssc;
using Ssc.SscLog;
using System;

namespace CSharpModule.Controllers {

    public class TestController : Controller {

        private static readonly Logger Logger = LogManager.GetLogger<TestController>(LogType.High);

        public TestController() {
            Initialize();
        }

        private void Initialize() {
            Register<ITestController>(
                (tc) => tc.Test(0, null, null,null),
                () => this.Test(0, null, null, null));

            Register<ITestController>(
                (tc) => tc.Test(0, null, null, null,null),
                () => this.Test(0, null, null, null,null));

            Register<ITestController>(
                (tc) => tc.Test(0, null, null, null, null,null),
                () => this.Test(0, null, null, null, null,null));
        }

        public bool Test(int num, string str, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str);
            var value = num + " " + str;
            // 请求客户端，客户端会收到两条信息，一个请求信息，一个当前方法返回信息
            Ssci.Invoke<ITestRequest>(() => (req) => req.TestRequest(value), peer);
            return true;
        }

        public bool Test(int num, string str, int[] array, IPeer peer, Action<Action> callback) {
            Test(num, str, peer, callback);

            foreach (var item in array) {
                Logger.Info(item);
            }
            return true;
        }

        public bool Test(int num, string str, int[] array, ITestPacket[] testPackets, IPeer peer, Action<Action> callback) {
            Test(num, str, array, peer, callback);

            foreach (var item in testPackets) {
                Logger.Info(item);
            }

            return true;
        }
    }
}