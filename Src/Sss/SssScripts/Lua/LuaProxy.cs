using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Ssc;
using Ssc.Ssc;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscTemplate;
using Ssm.Ssm;
using Sss.SssComponent;
using Sss.SssModule;
using Sss.SssRpc;
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    internal class LuaProxy {

        private static readonly Logger Logger = LogManager.GetLogger<LuaProxy>(LogType.Middle);

        public static Table GetObject(string interfaceName) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            return LuaPoolAllocator<ILuaPacket>.GetObject(interfaceName).Instance;
        }

        public static void Recycle(string interfaceName,ILuaPacket luaPacket) {
            if (luaPacket == null) {
                throw new ArgumentNullException(nameof(luaPacket));
            }

            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            LuaPoolAllocator<ILuaPacket>.Recycle(interfaceName, luaPacket);
        }
        
        public static LuaWrapper<ILuaPacket> New(string interfaceName, Table table, LuaHelper luaHelper) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }
            var luaPacket = ObjectFactory.GetActivator<ILuaPacket>(
                typeof(LuaPacket).GetConstructors().First())(table, luaHelper);
            return new LuaWrapper<ILuaPacket>(luaPacket);
        }

        public static string Register(Table instance,string id,Closure func) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }

            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            object LateBoundMethod(params object[] args) {
                var list = new List<object> { instance };

                var objs = LuaParser.Parse(args);
                list.AddRange(objs);

                return func.Call(list.ToArray()).ToObject();
            }

            RpcRegister.RegisterMethod(id, LateBoundMethod);
            return id;
        }

        public static LuaWrapper<LuaPeerComponent> NewPeerComponent(Table instance, LuaHelper luaHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            
            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            var luaPeerComponent = ObjectFactory.GetActivator<LuaPeerComponent>(
                typeof(LuaPeerComponent).GetConstructors().First())(instance, luaHelper);
            return new LuaWrapper<LuaPeerComponent>(luaPeerComponent);
        }
        
        public static LuaWrapper<LuaControllerComponent> NewRpcComponent(Table instance, LuaHelper luaHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            
            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            var luaRpcComponent = ObjectFactory.GetActivator<LuaControllerComponent>(
                typeof(LuaControllerComponent).GetConstructors().First())(instance, luaHelper);
            return new LuaWrapper<LuaControllerComponent>(luaRpcComponent);
        }

        public static LuaModule New(Table instance, LuaHelper luaHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            return ObjectFactory.GetActivator<LuaModule>(
                typeof(LuaModule).GetConstructors().First())(instance, luaHelper);
        }

        public static LuaController New(Table instance) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            return ObjectFactory.GetActivator<LuaController>(
                typeof(LuaController).GetConstructors().First())(instance);
        }

        public static void Invoke(string methodId,IPeer peer,Closure closure, params object[] objects) {
            if (string.IsNullOrEmpty(methodId)) {
                throw new ArgumentException(nameof(methodId));
            }

            if (peer == null) {
                throw new ArgumentNullException(nameof(peer));
            }

            ResponseCallback responseCallback = (rm, sd) => {
                closure?.Call(rm, sd);
            };

            Ssci.Invoke(methodId, peer, responseCallback, objects);
        }

        public static IUser Create() {
            return PoolAllocator<IUser>.GetObject();
        }
    }
}