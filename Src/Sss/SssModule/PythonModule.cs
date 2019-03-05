using System;
using System.Collections.Generic;
using IronPython.Runtime.Types;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssm.Ssm;
using Ssm.SsmManager;
using Ssm.SsmModule;
using Ssm.SsmService;
using Sss.SssScripts;
using Sss.SssScripts.Python;
using Sss.SssSerialization;
using Sss.SssSerialization.Python;

namespace Sss.SssModule {
    public class PythonModule : IModule {
        private static readonly Logger Logger = LogManager.GetLogger<PythonModule>(LogType.Middle);

        private ICacheManager _cacheManager;
        private IControllerComponentManager _controllerComponentManager;

        public List<string> RpcMethodIds { get; }
        public List<string> RpcPacketTypes {get;}

        public string ServiceId => _instance.ServiceId;
        public string ModuleName => PythonHelper.GetPythonTypeName(this);

        private IServer _server;
        private PythonHelper _pythonHelper;
        private dynamic _instance;

        public PythonModule(dynamic instance,PythonHelper pythonHelper) {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _pythonHelper = pythonHelper ?? throw new ArgumentNullException(nameof(pythonHelper));
            RpcMethodIds = new List<string>();
            RpcPacketTypes = new List<string>();
        }

        public virtual void Initialize(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _controllerComponentManager = controllerComponentManager ?? throw new ArgumentNullException(nameof(controllerComponentManager));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _instance.Initialize(server, cacheManager, controllerComponentManager);
        }

        public virtual void InitFinish(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
            _instance.InitFinish(server, cacheManager, controllerComponentManager);
        }

        public virtual void Dispose(ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            _instance.Dispose(cacheManager, controllerComponentManager);

            foreach (var item in RpcMethodIds) {
                RpcResponseManager.RemoveRpcMethod(item);
            }

            foreach (var item in RpcPacketTypes) {
                PacketManager.RemovePacket(item);
            }

            RpcMethodIds.Clear();
            RpcPacketTypes.Clear();

            _server.ClearModule(this);
        }

        public virtual void Finish(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            _instance.Finish(server, cacheManager, controllerComponentManager);
        }

        public virtual void Accepted(IUser peer, IReadStream readStream) {
            _instance.Accepted(peer, readStream);
        }

        public virtual void Connected(IUser peer, IReadStream readStream) {
            _instance.Connected(peer, readStream);
        }

        public virtual void Disconnected(IUser peer) {
            _instance.Disconnected(peer);
        }

        public void AddController(Func<dynamic> generator){
            if (generator == null) {
                throw new ArgumentNullException(nameof(generator));
            }

            var instance = generator.Invoke();
            if (instance == null) {
                throw new ArgumentNullException(nameof(generator));
            }
            var ids = instance.MethodIds as string;
            var splist = ids.Split(";");
            RpcMethodIds.AddRange(splist);
        }
        
        public void AddPacket(string interfaceName,dynamic implement){
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (implement == null) {
                throw new ArgumentNullException(nameof(implement));
            }

            if (PacketManager.Cantains(interfaceName)) {
                Logger.Warn($"已经缓存{interfaceName}");
                return;
            }

            ScriptPoolAllocator<IPythonPacket>.SetPool(interfaceName,arguments => {
                var instance = implement();
                var wrapper = instance.ISerializablePacket as ClassWrapper<IPythonPacket>;
                return wrapper.Value;
            });

            PacketManager.Register(interfaceName,typeof(IPythonPacket), 
                args => ScriptPoolAllocator<IPythonPacket>.GetObject(interfaceName,args));
            RpcPacketTypes.Add(interfaceName);
        }

        public void Dispose() {
            Dispose(_cacheManager, _controllerComponentManager);
        }
    }
}