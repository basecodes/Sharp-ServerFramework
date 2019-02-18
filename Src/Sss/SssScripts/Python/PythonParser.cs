using IronPython.Runtime;
using Ssc.SscLog;
using Sss.SssSerialization.Python;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sss.SssScripts.Python {
    internal sealed class PythonParser {
        private static readonly Logger Logger = LogManager.GetLogger<PythonParser>(LogType.Middle);

        public static object[] Parse(object[] args) {
            if (args == null) {
                return new object[0];
            }

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

                if (args[i] is IPythonPacket pythonPacket) {
                    objects[i] = ParsePacket(pythonPacket);
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

        private static object ParsePacket(IPythonPacket pythonPacket) {
            if (pythonPacket == null) {
                return null;
            }

            return pythonPacket.Instance;
        }

        private static object ParseArray(Array array) {
            if (array == null) {
                return null;
            }

            var type = array.GetType().GetElementType();
            if (typeof(IPythonPacket).IsAssignableFrom(type)) {
                var objects = new object[array.Length];
                for (int i = 0; i < array.Length; i++) {
                    objects[i] = (array.GetValue(i) as IPythonPacket).Instance;
                }
                return objects;
            }

            return array;
        }

        private static object ParseDictionary(IDictionary dicts) {
            if (dicts == null) {
                return null;
            }

            var dict = new PythonDictionary();
            foreach (DictionaryEntry item in dicts) {
                var key = item.Key;
                if (item.Key is IPythonPacket packetKey) {
                    key = packetKey.Instance;
                }

                var value = item.Value;
                if (item.Value is IPythonPacket packetValue) {
                    value = packetValue.Instance;
                }
                dict.Add(key, value);
            }

            return dict;
        }
    }
}
