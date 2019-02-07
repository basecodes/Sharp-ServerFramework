using System;
using System.Collections.Generic;
using System.Linq;
using Ssc.Ssc;
using Ssc.SscAttribute;
using Ssc.SscConfiguration;
using Ssc.SscExtension;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssc.SscUtil;
using Ssf.SsfComponent;
using Ssf.SsfManager;
using Ssf.SsfNetwork;
using Ssf.SsfNetwork.Messages;
using Ssf.SsfSecurity;
using Ssm.Ssm;
using Ssm.SsmComponent;
using Ssm.SsmModule;
using Ssm.SsmService;
using Sss.SssScripts.Lua;

namespace Ssf.SsfService {
    internal class Server : IServer {
        private static readonly Logger Logger = LogManager.GetLogger<Server>(LogType.Middle);

        private readonly object _lockEvents = new object();

        private readonly CacheManager _cacheManager;
        private readonly ModuleManager _moduleManager;
        private readonly RawMessageManager _rawMessageManager;
        private readonly ControllerComponentManager _controllerComponentManager;
        private readonly IServerSocket _serverSocket;

        private readonly Dictionary<string, Action<IUser, object>> _events;

        private Keys _keys;
        private Func<IUser,IReadStream,IWriteStream,bool> _acceptAction = (p,rs,ws) => true;
        private Action<IUser,IReadStream> _connectedAction = (p,rs) => { };
        private Action<IUser> _disconnectedAction = (p) => { };

        public string ID { get; }
        public List<string> RpcMethodIds { get; }
        public List<string> RpcPacketTypes { get; }

        public SocketConfig SocketConfig { get; private set; }

        public Server(string id, RawMessageManager rawMessageManager, ModuleManager moduleManager,
            CacheManager cacheManager, ControllerComponentManager controllerComponentManager) {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

            ID = id;
            LuaHelper.RegisterType<Server>();

            _moduleManager = moduleManager ?? throw new ArgumentNullException(nameof(moduleManager));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _controllerComponentManager = controllerComponentManager ?? throw new ArgumentNullException(nameof(controllerComponentManager));
            _rawMessageManager = rawMessageManager ?? throw new ArgumentNullException(nameof(rawMessageManager));

            _events = new Dictionary<string, Action<IUser, object>>();

            _serverSocket = new ServerSocket(id);
            _serverSocket.Accepted += AcceptConnected;
            _serverSocket.Connected += PeerConnected;
            _serverSocket.Disconnected += PeerDisconnected;

            RpcRegister.RegisterMethod(this);

            RpcMethodIds = new List<string>();
            RpcPacketTypes = new List<string>();

        }

        public void AddEventListener(string eventType, Action<IUser, object> listener) {
            if (string.IsNullOrEmpty(eventType)) throw new ArgumentNullException(nameof(eventType));

            if (listener == null) throw new ArgumentNullException(nameof(listener));

            if (_events.ContainsKey(eventType)) {
                Logger.Warn($"事件{eventType}已经注册过无需重复注册！");
                return;
            }

            lock (_lockEvents) {
                _events.Add(eventType, listener);
            }
        }

        public void PostNotification(string eventType, IUser player, object arg) {
            if (_events.TryGetValue(eventType, out var listener))
                listener.Invoke(player, arg);
            else
                Logger.Warn($"事件{eventType}未注册！");
        }

        public void RemoveEventListener(string eventType) {
            if (string.IsNullOrEmpty(eventType)) throw new ArgumentNullException(nameof(eventType));

            if (!_events.ContainsKey(eventType)) {
                Logger.Warn($"事件{eventType}已经删除无需重复删除！");
                return;
            }

            var success = false;
            lock (_lockEvents) {
                success = _events.Remove(eventType);
            }

            if (!success) Logger.Warn($"事件{eventType}删除失败！");
        }

        public void SetHandler(ushort opCode, Action<IUser, IDeserializable> action) {
            _rawMessageManager.AddRawMessage(opCode, action);
        }

        public void AddDependency<T>() where T : class,IModule {
            _moduleManager.AddModule<T>();
        }

        public void AddDependency<T>(params object[] args) where T : class,IModule {
            _moduleManager.AddModule<T>(args);
        }
        
        [RpcResponse("AAAA295F-78EB-4D17-A91E-8895A29F8D45",typeof(void),RpcType.System)]
        private void HandleRawMessage(RawMessage rawMessage,IUser user,Action<Action> callback) {
            var action = _rawMessageManager.GetRawMessageHandler(rawMessage.OpCode);
            if (action == null) {
                Logger.Warn($"操作码{rawMessage.OpCode}处理程序未注册！");
                return;
            }

            action.Invoke(user, rawMessage.Deserializable);
        }

        private void DelegateReverseInvoke() {
            var list = _disconnectedAction.GetInvocationList();
            _disconnectedAction = (p)=>{ };
            foreach (var item in list.Reverse()) {
                var temp = item as Action<IUser>;
                _disconnectedAction += temp;
            }
        }

        public virtual void StartServer(SocketConfig socketConfig) {
            SocketConfig = socketConfig;
            foreach (var module in _moduleManager.ForeachInitializedModule()) {
                var serverModule = module.Value;
                if (serverModule.ServiceId != ID) {
                    continue;
                }
                RpcMethodIds.AddRange(serverModule.RpcMethodIds);
                RpcPacketTypes.AddRange(serverModule.RpcPacketTypes);
            }

            _serverSocket.Start(socketConfig);
        }

        public virtual void StopServer() {
            _serverSocket.Stop();
            _events.Clear();
        }

        public virtual void Connect(SocketConfig socketConfig) {
            SocketConfig = socketConfig;
            foreach (var module in _moduleManager.ForeachInitializedModule()) {
                var serverModule = module.Value;
                if (serverModule.ServiceId != ID) {
                    continue;
                }
                RpcMethodIds.AddRange(serverModule.RpcMethodIds);
                RpcPacketTypes.AddRange(serverModule.RpcPacketTypes);
            }

            _keys = Ssfi.Security.CreateAesKey();

            using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                writeStream.ShiftRight(_keys.PublicKey);
                _serverSocket.Connect(socketConfig, writeStream);
            }
        }

        private void PeerConnected(IUser peer, IReadStream readStream) {

            var length = readStream.ShiftRight<ushort>();
            var data = readStream.ShiftRight(length);
            var bytes = new byte[data.Count];
            Buffer.BlockCopy(data.Buffer, data.Offset, bytes, 0, bytes.Length);
            var buffer = Ssfi.Security.DecryptAesKey(bytes, _keys);
            var aesKey = buffer.toString();

            var securityComponent = SecurityComponent.GetObject();
            securityComponent.AesKey = aesKey;
            peer.AddComponent<ISecurityComponent>(securityComponent);
            Logger.Debug($"添加{nameof(ISecurityComponent)}组件！");

            try {
                _connectedAction?.Invoke(peer,readStream);
            } catch (Exception e) {
                Logger.Error(e);
            }
        }

        private bool AcceptConnected(IUser peer, IReadStream readStream, IWriteStream writeStream) {
            if (writeStream == null) {
                Logger.Warn($"{nameof(writeStream)}为空！");
                return true;
            }

            var publicKey = readStream.ShiftRight<string>();
            var key = ID +"_" + SystemUtil.CreateRandomString(Ssfi.CryptoConfig.CryptonKeyLength);
            var encryptKey = Ssfi.Security.EncryptAesKey(key, publicKey);

            var securityComponent = SecurityComponent.GetObject();
            securityComponent.AesKey = key;
            securityComponent.Encryptkey = encryptKey;

            peer.AddComponent<ISecurityComponent>(securityComponent);
            Logger.Debug($"添加{nameof(ISecurityComponent)}组件！");

            writeStream.ShiftRight((ushort) encryptKey.Length);
            Logger.Debug("EncryptKey Length:" + encryptKey.Length);
            writeStream.ShiftRight(encryptKey);
            writeStream.ShiftRight(string.Join(";", RpcMethodIds));

            var success = true;
            try {
                success = _acceptAction == null? true:_acceptAction.Invoke(peer,readStream,writeStream);
            } catch (Exception e) {
                Logger.Error(e);
            }
            return success;
        }

        private void PeerDisconnected(IUser peer) {
            if (peer == null) {
                Logger.Warn($"{nameof(peer)}为空！");
                return;
            }

            try {
                _disconnectedAction?.Invoke(peer);
            } catch (Exception e) {
                Logger.Error(e);
            }

            using (var securityComponent = peer.GetComponent<ISecurityComponent>()) {
                peer.RemoveComponent<ISecurityComponent>();
                Logger.Debug($"移除{nameof(ISecurityComponent)}组件！");
            }
        }

        public void InitializeModules() {
            foreach (var module in _moduleManager.ForeachUninitializedModule()) {
                var serverModule = module.Value;
                if (serverModule.ServiceId == ID) {
                    serverModule.Initialize(this, _cacheManager, _controllerComponentManager);
                    LuaHelper.RegisterType(serverModule.GetType());

                    _acceptAction += serverModule.Accepted;
                    _connectedAction += serverModule.Connected;
                    _disconnectedAction += serverModule.Disconnected;

                    _moduleManager.ChangeUninitializedModuleState(module.Key);
                }
            }

            DelegateReverseInvoke();

            foreach (var module in _moduleManager.ForeachInitializedModule()) {
                var serverModule = module.Value;
                if (serverModule.ServiceId == ID) {
                    serverModule.InitFinish(this, _cacheManager, _controllerComponentManager);
                }
            }
        }

        public void ClearModule(IModule module) {
            if (module == null) {
                throw new ArgumentNullException(nameof(module));
            }

            _connectedAction -= module.Connected;
            _disconnectedAction -= module.Disconnected;
            _acceptAction -= module.Accepted;

            foreach (var item in module.RpcMethodIds) {
                RpcMethodIds.Remove(item);
            }

            foreach (var item in module.RpcPacketTypes) {
                RpcPacketTypes.Remove(item);
            }
        }
    }
}