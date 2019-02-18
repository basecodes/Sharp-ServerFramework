using Ssc.Ssc;
using Ssc.SscLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpModule.Controllers {

    public class TestController : Controller {

        private static readonly Logger Logger = LogManager.GetLogger<TestController>(LogType.High);

        public TestController() {
            Initialize();
        }

        private void Initialize() {
            Register<ITestResponse>(
                (tc) => tc.TestResponse(0, null, null, null),
                () => this.Test(0, null, null, null));

            Register<ITestResponse>(
                (tc) => tc.TestResponse(0, null, null, null, null),
                () => this.Test(0, null, null, null, null));

            Register<ITestResponse>(
                (tc) => tc.TestResponse(0, null, null, null, null, null),
                () => this.Test(0, null, null, null, null, null));

            Register<ITestResponse>(
                (tc) => tc.TestResponse(null, null, null, null),
                () => this.Test(null, null, null, null));
        }

        public bool Test(int num, string str, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str);

            Invoke<ITestRequest>(() => (req) => req.TestRequest(num,str), peer);
            return true;
        }

        public bool Test(int num, string str, int[] array, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str); Logger.Info(num);
            Logger.Info(str);

            foreach (var item in array) {
                Logger.Info(item);
            }

            Invoke<ITestRequest>(() => (req) => req.TestRequest(num,str,array), peer);
            return true;
        }

        public bool Test(int num, string str, int[] array, ITestPacket[] testPackets, IPeer peer, Action<Action> callback) {
            Logger.Info(num);
            Logger.Info(str);

            foreach (var item in array) {
                Logger.Info(item);
            }

            foreach (var item in testPackets) {
                Logger.Info(item);
            }

            Invoke<ITestRequest>(() => (req) => req.TestRequest(num, str,array,testPackets), peer);
            return true;
        }

        public bool Test(string str, Dictionary<int, ITestPacket> dict, IPeer peer, Action<Action> action) {
            Logger.Info(str);

            foreach (var item in dict) {
                Logger.Info(item.Key + " " + item.Value);
            }

            Invoke<ITestRequest>(() => (req) => req.TestRequest(str, dict), peer);
            return true;
        }
    }
}