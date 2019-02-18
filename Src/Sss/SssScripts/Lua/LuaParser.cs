using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    internal sealed class LuaParser {
        private static readonly Logger Logger = LogManager.GetLogger<LuaParser>(LogType.Middle);

        public static object[] Parse(object[] args,Script script) {
            if (args == null) {
                return new object[0];
            }

            var objects = new object[args.Length];
            for (var i = 0; i < args.Length; i++) {
                if (args[i] is IConvertible value) {
                    var table = DynValue.NewTable(script).Table;
                    table["Type"] = FieldType.BaseType;
                    table["TypeCode"] = value.GetTypeCode();
                    table["Value"] = value;
                    objects[i] = table;
                    continue;
                }

                if (args[i] is Array array) {
                    objects[i] = ParseArray(array,script);
                    continue;
                }

                if (args[i] is LuaPacket luaPacket) {
                    objects[i] = ParsePacket(luaPacket,script);
                    continue;
                }

                if (args[i] is IDictionary dict) {
                    objects[i] = ParseDictionary(dict,script);
                    continue;
                }

                objects[i] = args[i];
            }

            return objects;
        }

        private static object ParsePacket(LuaPacket luaPacket,Script script) {
            if (luaPacket == null) {
                return null;
            }

            var table = DynValue.NewTable(script).Table;
            table["Type"] = FieldType.PacketType;
            table["Value"] = luaPacket.Instance;
            return table;
        }

        private static object ParseArray(Array array,Script script) {
            if (array == null) {
                return null;
            }

            var table = DynValue.NewTable(script).Table;
            var values = DynValue.NewTable(script).Table;
            var type = FieldType.ArrayBase;
            for (var i = 0; i < array.Length; i++) {
                var value = array.GetValue(i);
                if (value is LuaPacket packet) {
                    values.Append(DynValue.NewTable(packet.Instance));
                    type = FieldType.ArrayPacket;
                } else {
                    values.Append(DynValue.FromObject(script,value));
                }
            }

            table["Type"] = type;
            table["Value"] = values;
            table["ElementTypeCode"] = Type.GetTypeCode(array.GetType().GetElementType());
            return table;
        }

        private static object ParseDictionary(IDictionary dicts,Script script) {
            if (dicts == null) {
                return null;
            }

            var table = DynValue.NewTable(script).Table;
            var values = DynValue.NewTable(script).Table;
            var offset = FieldType.DictKBVB;

            foreach (DictionaryEntry item in dicts) {
                var key = item.Key;
                if (item.Key is LuaPacket packetKey) {
                    key = packetKey.Instance;
                    offset = FieldType.DictKBVB + 2;
                }

                var value = item.Value;
                if (item.Value is LuaPacket packetValue) {
                    value = packetValue.Instance;
                    offset = FieldType.DictKBVB + 1;
                }
                values[key] = value;
            }

            var types = dicts.GetType().GenericTypeArguments;
            table["KeyType"] = types[0];
            table["ValueType"] = types[1];
            table["Type"] = offset;
            table["Value"] = values;
            return table;
        }
    }
}