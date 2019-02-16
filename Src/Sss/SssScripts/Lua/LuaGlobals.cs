using System;
using MoonSharp.Interpreter;
using Ssc.SscSerialization;
using Sss.SssRpc;
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    public class LuaGlobals {
        public static void Init() {
            GlobalType();
        }

        private static void GlobalType() {
            LuaHelper.RegisterType<LuaController>();
            LuaHelper.RegisterType<LuaPacket>();
            LuaHelper.RegisterType<LuaWrapper<ILuaPacket>>();

            ClrToScript.RegisterUserDataToTable();
            ClrToScript.RegisterPeerComponentToTable();
            ClrToScript.RegisterRpcComponentToTable();
            
            ScriptToClr.RegisterRpcServiceToTable();
            ScriptToClr.RegisterUserData<TypeCode>();
            ScriptToClr.RegisterBaseType(DataType.Number);
            ScriptToClr.RegisterBaseType(DataType.Boolean);
            ScriptToClr.RegisterBaseType(DataType.String);
            ScriptToClr.RegisterBaseType(DataType.Nil);
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