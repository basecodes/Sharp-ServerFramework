using System;
using Ssc.SscConfiguration;
using Ssc.SscLog;
using Ssf.SsfManager;

namespace Ssf.SsfService {
    internal class MasterServer : Server {
        private static readonly Logger Logger = LogManager.GetLogger<MasterServer>(LogType.Middle);

        public MasterServer(string id, RawMessageManager rawMessageManager, ModuleManager moduleManager,
            CacheManager cacheManager, ControllerComponentManager controllerComponentManager)
            : base(id, rawMessageManager, moduleManager, cacheManager, controllerComponentManager) {
        }

        public bool IsMasterRunning { get; protected set; }

        public event Action<MasterServer> MasterStarted;

        public event Action<MasterServer> MasterStopped;

        public override void Connect(SocketConfig socketConfig) {
            if (IsMasterRunning) return;
            base.Connect(socketConfig);

            IsMasterRunning = true;
            MasterStarted?.Invoke(this);
        }

        public override void StartServer(SocketConfig socketConfig) {
            if (IsMasterRunning) return;
            base.StartServer(socketConfig);

            IsMasterRunning = true;
            MasterStarted?.Invoke(this);
        }

        public override void StopServer() {
            if (!IsMasterRunning) return;
            base.StopServer();

            IsMasterRunning = false;
            MasterStopped?.Invoke(this);
        }
    }
}