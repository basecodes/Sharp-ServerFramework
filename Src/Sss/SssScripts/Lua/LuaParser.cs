using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Sss.SssSerialization;

namespace Sss.SssScripts.Lua {
    internal class LuaParser {
        private static readonly Logger Logger = LogManager.GetLogger<LuaParser>(LogType.Middle);

        public static object[] Parse(object[] args) {
            if (args == null) return new object[0];

            var objects = new object[args.Length];
            for (var i = 0; i < args.Length; i++) {
                if (args[i] is IConvertible) {
                    objects[i] = args[i];
                    continue;
                }

                if (args[i] is Array array) {
                    objects[i] = ParseArray(array);
                    continue;
                }

                if (args[i] is LuaPacket luaPacket) {
                    objects[i] = ParsePacket(luaPacket);
                    continue;
                }

                if (args[i] is IDictionary dict) {
                    objects[i] = ParseDictionary(dict);
                    continue;
                }

                objects[i] = args[i];
            }

            return objects;
        }

        private static object ParsePacket(LuaPacket luaPacket) {
            if (luaPacket == null) return null;
            return luaPacket.Instance;
        }

        private static object ParseArray(Array array) {
            if (array == null) return null;

            var elementType = array.GetType().GetElementType();
            if (typeof(ILuaPacket) == elementType) {
                var lsps = new Table[array.Length];
                for (var i = 0; i < lsps.Length; i++)
                    if (array.GetValue(i) is LuaPacket tmp)
                        lsps[i] = tmp.Instance;

                return lsps;
            }

            if (typeof(IConvertible).IsAssignableFrom(elementType)) return array;

            return null;
        }

        private static object ParseDictionary(IDictionary dicts) {
            if (dicts == null) return null;
            var types = dicts.GetType().GetGenericArguments();

            var count = types.Count(t => typeof(IConvertible).IsAssignableFrom(t) || typeof(ISerializablePacket).IsAssignableFrom(t));
            if (count == types.Length) {
                var dictType = typeof(Dictionary<,>).MakeGenericType(
                    typeof(ISerializablePacket).IsAssignableFrom(types[0]) ? typeof(Table) : types[0],
                    typeof(ISerializablePacket).IsAssignableFrom(types[1]) ? typeof(Table) : types[1]);

                var dict = ObjectFactory.GetActivator<IDictionary>(dictType.GetConstructors().First())();

                foreach (DictionaryEntry value in dicts) {
                    var key = value.Key;
                    if (value.Key is ISerializablePacket)
                        if (value.Key is LuaPacket tmp)
                            key = tmp.Instance;
                    var vle = value.Value;
                    if (value.Value is ISerializablePacket)
                        if (value.Value is LuaPacket tmp)
                            vle = tmp.Instance;
                    dict.Add(key, vle);
                }

                return dict;
            }

            return null;
        }
    }
}