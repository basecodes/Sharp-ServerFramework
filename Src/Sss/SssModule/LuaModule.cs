using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Ssc;
using Ssc.Ssc;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssm.Ssm;
using Ssm.SsmManager;
using Ssm.SsmModule;
using Ssm.SsmService;
using Sss.SssScripts.Lua;
using Sss.SssSerialization;

namespace Sss.SssModule {
    internal class LuaModule : IModule {
        private static readonly Logger Logger = LogManager.GetLogger<LuaModule>(LogType.Middle);

        public List<string> RpcMethodIds {get;}
        public List<string> RpcPacketTypes { get; }

        private ICacheManager _cacheManager;
        private readonly LuaHelper _luaHelper;
        private IControllerComponentManager _controllerComponentManager;
        private readonly Table _table;

        private IServer _server;
        public string ServiceId => LuaHelper.Get(_table, nameof(ServiceId)).ToObject<string>();

        public string ModuleName => LuaHelper.Get(_table,nameof(ModuleName)).ToObject<string>();

        public LuaModule(Table table, LuaHelper luaHelper) {
            
            _table = table ?? throw new ArgumentNullException(nameof(table));
            _luaHelper = luaHelper ?? throw new ArgumentNullException(nameof(luaHelper));

            RpcMethodIds = new List<string>();
            RpcPacketTypes = new List<string>();
            
            luaHelper.UserStatic<Ssci>();

            LuaHelper.RegisterType(GetType());
            LuaHelper.RegisterType<IReadStream>();
            LuaHelper.RegisterType<IWriteStream>();
        }

        public virtual bool Accepted(IUser peer, IReadStream readStream, IWriteStream writeStream) {
            _luaHelper.Call(_table, nameof(Accepted), _table, peer,readStream,writeStream);
            return true;
        }

        public virtual void Connected(IUser peer, IReadStream readStream) {
             _luaHelper.Call(_table, nameof(Connected), _table, peer,readStream);
        }

        public virtual void Disconnected(IUser peer) {
            _luaHelper.Call(_table, nameof(Disconnected), _table, peer);
        }
        public void Initialize(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _controllerComponentManager = controllerComponentManager ?? throw new ArgumentNullException(nameof(controllerComponentManager));

            _luaHelper.Call(_table, nameof(Initialize), _table, server, cacheManager, controllerComponentManager);
        }

        public virtual void InitFinish(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
            _luaHelper.Call(_table, nameof(InitFinish), _table, server, cacheManager, controllerComponentManager);
        }

        public virtual void Dispose(ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            _luaHelper.Call(_table, nameof(Dispose), _table, cacheManager, controllerComponentManager);

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

        public void Finish(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
            _luaHelper.Call(_table, nameof(Finish), _table, server, cacheManager, controllerComponentManager);
        }

        public void AddController(Closure newTable) {
            if (newTable == null ) {
                throw new ArgumentNullException(nameof(newTable));
            }

            var table = newTable.Call();
            if (table == DynValue.Nil) {
                throw new ArgumentException(nameof(newTable));
            }

            var dynValue = table.Table.Get(nameof(IController));
            var rpcObject = dynValue.ToObject<IController>();
 
            var keys = table.Table.Get("MethodIds");
            if (keys == DynValue.Nil) {
                throw new Exception($"无MethodIds！");
            }

            var ids = keys.ToObject<string>();
            var splist = ids.Split(";");
            RpcMethodIds.AddRange(splist);
        }


        public void AddPacket(string interfaceName, Closure newTable) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentException(nameof(interfaceName));
            }

            if (newTable == null) {
                throw new ArgumentNullException(nameof(newTable));
            }

            if (PacketManager.Cantains(interfaceName)) {
                Logger.Warn($"已经缓存{interfaceName} 和 {nameof(Table)}");
                return;
            }

            LuaPoolAllocator<ILuaPacket>.CreateObjectPool(interfaceName, args => {
                var dynValue = newTable.Call();
                if (Equals(dynValue, DynValue.Nil)) {
                    throw new DynamicCreateObjectException($"动态创建{interfaceName}表失败!");
                }
                var table = dynValue.Table;
                return table.Get(nameof(ISerializablePacket)).ToObject<LuaWrapper<ILuaPacket>>().Value;
            });

            PacketManager.Register(interfaceName, typeof(ILuaPacket),
                args => LuaPoolAllocator<ILuaPacket>.GetObject(interfaceName));

            RpcPacketTypes.Add(interfaceName);
        }

        public void Dispose() {
            Dispose(_cacheManager, _controllerComponentManager);
        }
    }
}