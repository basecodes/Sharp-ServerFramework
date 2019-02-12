﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IronPython.Runtime;
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
using Sss.SssScripts.Lua;
using Sss.SssScripts.Pythons;

namespace Sss.SssModule {
    public abstract class PythonModule : IModule {
        private static readonly Logger Logger = LogManager.GetLogger<PythonModule>(LogType.Middle);

        private ICacheManager _cacheManager;
        private IControllerComponentManager _controllerComponentManager;

        public List<string> RpcMethodIds { get; }
        public List<string> RpcPacketTypes {get;}

        private IServer _server;
        private PythonHelper _pythonHelper;
        public PythonModule() {
            RpcMethodIds = new List<string>();
            RpcPacketTypes = new List<string>();
        }

        protected void SetPythonHelper(PythonHelper pythonHelper) {
            _pythonHelper = pythonHelper ?? throw new ArgumentNullException(nameof(pythonHelper));
        }

        public abstract string ServiceId { get; }

        public virtual void Initialize(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _controllerComponentManager = controllerComponentManager ?? throw new ArgumentNullException(nameof(controllerComponentManager));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public virtual void InitFinish(IServer server, ICacheManager cacheManager,
            IControllerComponentManager controllerComponentManager) {
            Console.WriteLine();
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

            _server.ClearModule(this);
        }

        public virtual void Finish(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager) {
        }

        public virtual bool Accepted(IUser peer, IReadStream readStream, IWriteStream writeStream) {
            return true;
        }

        public virtual void Connected(IUser peer, IReadStream readStream) {
        }

        public virtual void Disconnected(IUser peer) {
        }

        public void AddController(Func<dynamic> generator){
            if (generator == null) {
                throw new ArgumentNullException(nameof(generator));
            }

            var instance = generator.Invoke();
            if (instance == null) {
                throw new ArgumentException(nameof(generator));
            }
            var ids = instance.MethodIds as string;
            var splist = ids.Split(";");
            RpcMethodIds.AddRange(splist);
        }

        protected void SetObjectPool(Type type, dynamic implement){
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            PoolAllocator<IPythonFactory>.SetPool(arguments => implement());
            LuaHelper.RegisterType(type);
        }
        
        protected void SetPacketPool(ISerializablePacket serializablePacket,dynamic implement){
            if (serializablePacket == null) {
                throw new ArgumentNullException(nameof(serializablePacket));
            }

            if (!(serializablePacket is IFactory)) {
                throw new ArgumentException($"为实现{nameof(IFactory)}接口！");
            }

            var interfaceType = serializablePacket.GetType();

            PoolAllocator<IPythonSerializable>.SetPool(arguments => implement());
            PacketManager.Register(interfaceType,args => (
                PoolAllocator<IPythonSerializable>.GetObject(args)) as ISerializablePacket);
            LuaHelper.RegisterType(interfaceType);
        }
        
        public void AddPacket(PythonType interfaceType,dynamic implement){
            if (interfaceType == null) {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            if (implement == null) {
                throw new ArgumentNullException(nameof(implement));
            }

            var interfaceName = PythonHelper.GetPythonTypeName(interfaceType);
            if (PacketManager.Cantains(interfaceName)) {
                Logger.Warn($"已经缓存{interfaceName}");
                return;
            }

            PacketManager.Register(interfaceName,interfaceType.__clrtype__(),args => implement());

            RpcPacketTypes.Add(interfaceName);
            LuaHelper.RegisterType(interfaceType);
        }

        public void Dispose() {
            Dispose(_cacheManager, _controllerComponentManager);
        }
    }
}