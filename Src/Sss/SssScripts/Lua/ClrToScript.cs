using MoonSharp.Interpreter;
using Ssc.SscSerialization;
using Sss.SssComponent;
using Sss.SssSerialization.Lua;
using System;
using System.Collections;

namespace Sss.SssScripts.Lua {
    internal static class ClrToScript {

        public static void RegisterPeerComponentToTable() {
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(
                typeof(LuaPeerComponent),
                (v, o) => {
                    if (o is ILuaComponent luaComponent) {
                        return DynValue.NewTable(luaComponent.Instance);
                    }
                    return DynValue.Nil;
                }
            );
        }

        public static void RegisterRpcComponentToTable() {
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(
                typeof(LuaControllerComponent),
                (v, o) => {
                    if (o is ILuaComponent luaComponent) {
                        return DynValue.NewTable(luaComponent.Instance);
                    }
                    return DynValue.Nil;
                }
            );
        }

        public static void RegisterUserDataToTable() {
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(
                typeof(LuaPacket),
                (v, o) => {
                    if (o is LuaPacket luaPacket) {
                        if (Equals(luaPacket.Instance.Get(nameof(ISerializablePacket)), DynValue.Nil)) {
                            return UserData.Create(o);
                        }
                        return DynValue.NewTable(luaPacket.Instance);
                    }

                    return DynValue.Nil;
                }
            );
        }
    }
}