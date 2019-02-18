using System;
using MoonSharp.Interpreter;
using Ssc.SscSerialization;
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    public class LuaGlobals {
        public static void Init() {
            GlobalType();
        }

        private static void GlobalType() {
            LuaHelper.RegisterType<LuaController>();
            LuaHelper.RegisterType<LuaPacket>();
            LuaHelper.RegisterType<ClassWrapper<ILuaPacket>>();

            ClrToScript.RegisterUserDataToTable();
            ClrToScript.RegisterPeerComponentToTable();
            ClrToScript.RegisterRpcComponentToTable();
            
            ScriptToClr.RegisterControllerToTable();
            ScriptToClr.RegisterUserData<TypeCode>();
            ScriptToClr.RegisterTableToBaseType();
            ScriptToClr.RegisterPacket();
            ScriptToClr.RegisterTableToArray<IConvertible>();
            ScriptToClr.RegisterTableToArray<ISerializablePacket>();
            ScriptToClr.RegisterTableToDictionary<IConvertible, IConvertible>();
            ScriptToClr.RegisterTableToDictionary<IConvertible, ISerializablePacket>();
            ScriptToClr.RegisterTableToDictionary<ISerializablePacket, ISerializablePacket>();
            ScriptToClr.RegisterTableToDictionary<ISerializablePacket, IConvertible>();
            ScriptToClr.RegisterTableToPeerComponent();
            ScriptToClr.RegisterTableToObject();
            ScriptToClr.RegisterTableToRpcComponent();
            ScriptToClr.RegisterFunctionToResponseCallback();
            ScriptToClr.RegisterFunctionToAction();
        }
    }
}