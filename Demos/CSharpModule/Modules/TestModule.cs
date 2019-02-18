
using CSharpModule.Controllers;
using CSharpModule.Packets;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssm.Ssm;
using Ssm.SsmManager;
using Ssm.SsmModule;
using Ssm.SsmService;

namespace CSharpModule.Modules {
    public class TestModule : Module {
        public override string ServiceId => "Test";
        private static readonly Logger Logger = LogManager.GetLogger<TestModule>(LogType.High);
        public override void Initialize(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            base.Initialize(server, cacheManager, controllerComponentManager);

            AddController(()=> new TestController());
            AddPacket<ITestPacket, TestPacket>();
        }

        public override void Connected(IUser peer, IReadStream readStream) {
            base.Connected(peer, readStream);
            Logger.Debug("用户连入！");
        }
    }
}
