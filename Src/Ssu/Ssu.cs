using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ssc;
using Ssc.Ssc;
using Ssc.SscAttribute;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscSerialization;
using Ssu.Ssu;
using Ssu.SsuManager;
using Ssu.SsuSecurity;

namespace Ssu {
    public class Ssui {
        
        internal static ISecurity Security { get; }
        internal static Dictionary<string, IPeer> Connections { get; }
        
        private static readonly Logger Logger = LogManager.GetLogger<Ssui>(LogType.Middle);
        
        static Ssui() {
            Security = new Security();
            Connections = new Dictionary<string, IPeer>();
        }

        internal static void Initialize() {
            Ssc.Ssci.Initialize();
            var logAppenders = new SsuLogAppenders();
            LogManager.Initialize(logAppenders.ConsoleAppender);
        }

        public static void RegisterPacket(Assembly assembly) {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = assembly.GetTypes();
            foreach (var type in types) {
                var rpcPacket = type.GetCustomAttribute<RpcPacketAttribute>();
                if (rpcPacket != null) {
                    RegisterPacket(rpcPacket.BaseType, type);
                }
            }
        }

        public static void Register(ushort opCode, Action<IPeer, ISerializable> action) {
            RawMessageManager.AddRawMessage(opCode, action);
        }

        public static void RegisterPacket<Interface, Implement>()
            where Interface : class 
            where Implement : ISerializablePacket, Interface {
            
            PacketManager.Register<Interface>(
                args => ObjectFactory.GetActivator<ISerializablePacket>(typeof(Implement).GetConstructors().First())());
        }

        public static void RegisterPacket(Type interfaceType, Type implementType) {
            if (interfaceType == null) {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementType == null) {
                throw new ArgumentNullException(nameof(implementType));
            }

            PacketManager.Register(interfaceType,
                args => ObjectFactory.GetActivator<ISerializablePacket>(implementType.GetConstructors().First())());
        }

        public void Invoke<T>(Func<Expression<Action<T>>> func, ResponseCallback responseCallback) {
            var methodTuple = RpcRequestManager.CreateMethodTuple(func);
            var methodId = methodTuple.RpcMethod.GetId();
            if (!Connections.TryGetValue(methodId, out var peer)) {
                Logger.Warn($"服务器端没有注册{methodId}方法！");
                return;
            }

            Ssci.Invoke(func, peer, responseCallback);
        }

        public static void Invoke<T>(Func<Expression<Action<T>>> func) {
            var methodTuple = RpcRequestManager.CreateMethodTuple(func);
            var methodId = methodTuple.RpcMethod.GetId();
            if (!Connections.TryGetValue(methodId, out var peer)) {
                Logger.Warn($"服务器端没有注册{methodId}方法！");
                return;
            }

            Ssci.Invoke(func, peer, null);
        }

        public static void Invoke(string methodId, IPeer peer, ResponseCallback responseCallback, Action<ISerializable> action) {
            Ssci.Invoke(methodId, peer, responseCallback, action);
        }
        public static void Invoke(string methodId, IPeer peer, Action<ISerializable> action) {
            Invoke(methodId, peer, null, action);
        }

        internal static void Recycle<Interface, Implement>(Implement implement) where Interface : class {
            Recycle(typeof(Interface), implement);
        }

        internal static void Recycle<Implement>(Type interfaceType, Implement implement) {
            Recycle(interfaceType, implement as object);
        }
    }
}