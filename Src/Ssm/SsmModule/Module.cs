using System;
using System.Collections.Generic;
using System.Linq;
using Ssc.Ssc;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssm.Ssm;
using Ssm.SsmManager;
using Ssm.SsmService;

namespace Ssm.SsmModule {
    
    public abstract class Module : IModule {
        private static readonly Logger Logger = LogManager.GetLogger<Module>(LogType.Middle);

        public abstract string ServiceId { get; }
        public List<string> RpcMethodIds { get; }
        public List<string> RpcPacketTypes { get; }

        public string ModuleName => GetType().Name;

        private ICacheManager _cacheManager;
        private IControllerComponentManager _controllerComponentManager;

        private IServer _server;
        public Module() {
            RpcMethodIds = new List<string>();
            RpcPacketTypes = new List<string>();
        }

        public virtual bool Accepted(IUser peer, IReadStream readStream, IWriteStream writeStream) {
            return true;
        }

        public virtual void Connected(IUser peer, IReadStream readStream) {
        }

        public virtual void Disconnected(IUser peer) {
        }

        public virtual void Initialize(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _controllerComponentManager = controllerComponentManager ?? throw new ArgumentNullException(nameof(controllerComponentManager));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public virtual void InitFinish(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
        }

        public virtual void Dispose(ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            foreach (var item in RpcMethodIds) {
                RpcResponseManager.RemoveRpcMethod(item);
            }

            foreach (var item in RpcPacketTypes) {
                PacketManager.RemovePacket(item);
            }

            RpcMethodIds.Clear();
            RpcPacketTypes.Clear();

            _server?.ClearModule(this);
        }

        public virtual void Finish(IServer server, 
            ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
        }

        protected void AddController<Implement>(Func<Implement> generator)
            where Implement:IController {
            if (generator == null) {
                throw new ArgumentNullException(nameof(generator));
            }

            var implement = generator.Invoke();
            RpcMethodIds.AddRange(implement.MethodIds);
        }

        protected void SetObjectPool<Interface, Implement>()
            where Interface : IMemoryable
            where Implement : Interface {
                        
            PoolAllocator<Interface>.SetPool(
                arguments => ObjectFactory.GetActivator<Interface>( 
                    typeof(Implement).GetConstructors().First())(arguments));
        }

        protected void AddPacket<Interface, Implement>()
            where Interface : class,ISerializablePacket
            where Implement : Interface,new(){
            
            if (PacketManager.Cantains(typeof(Interface).Name)) {
                Logger.Warn($"已经缓存{typeof(Interface).Name} 和 {typeof(Implement).Name}");
                return;
            }

            SetObjectPool<Interface, Implement>();

            PacketManager.Register<Interface>(args => PoolAllocator<Interface>.GetObject(args));

            RpcPacketTypes.Add(typeof(Interface).Name);
        }

        public override string ToString() {
            return GetType().Name;
        }

        public void Dispose() {
            Dispose(_cacheManager, _controllerComponentManager);
        }
    }
}