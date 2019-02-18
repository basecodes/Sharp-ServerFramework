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
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    internal class LuaProxy {

        private static readonly Logger Logger = LogManager.GetLogger<LuaProxy>(LogType.Middle);

        
        public static ClassWrapper<ILuaPacket> CreatePacket(string interfaceName, Table table, LuaHelper luaHelper) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (table == null) {
                throw new ArgumentNullException(nameof(table));
            }

            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            var luaPacket = ObjectFactory.GetActivator<ILuaPacket>(
                typeof(LuaPacket).GetConstructors().First())(interfaceName,table, luaHelper);
            return new ClassWrapper<ILuaPacket>(luaPacket);
        }

        public static ClassWrapper<LuaPeerComponent> CreatePeerComponent(Table instance, LuaHelper luaHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            
            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            var luaPeerComponent = ObjectFactory.GetActivator<LuaPeerComponent>(
                typeof(LuaPeerComponent).GetConstructors().First())(instance, luaHelper);
            return new ClassWrapper<LuaPeerComponent>(luaPeerComponent);
        }
        
        public static ClassWrapper<LuaControllerComponent> CreateControllerComponent(Table instance, LuaHelper luaHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            
            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            var luaRpcComponent = ObjectFactory.GetActivator<LuaControllerComponent>(
                typeof(LuaControllerComponent).GetConstructors().First())(instance, luaHelper);
            return new ClassWrapper<LuaControllerComponent>(luaRpcComponent);
        }

        public static LuaModule CreateModule(Table instance, LuaHelper luaHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            if (luaHelper == null) {
                throw new ArgumentNullException(nameof(luaHelper));
            }

            return ObjectFactory.GetActivator<LuaModule>(
                typeof(LuaModule).GetConstructors().First())(instance, luaHelper);
        }

        public static LuaController CreateController(Table instance) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            return ObjectFactory.GetActivator<LuaController>(
                typeof(LuaController).GetConstructors().First())(instance);
        }

        public static IUser Create() {
            return PoolAllocator<IUser>.GetObject();
        }
    }
}