using IronPython.Runtime.Types;
using Ssc;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscRpc;
using Sss.SssSerialization.Python;
using System;
using System.Collections;
using Ssc.SscSerialization;
using Ssc.SscExtension;

namespace Sss.SssScripts.Python {
    public sealed class PythonController : Controller {
        private static readonly Logger Logger = LogManager.GetLogger<PythonController>(LogType.Middle);

        public dynamic Instance { get; }

        public PythonController(dynamic instance) {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public void Invoke(string methodId, IPeer peer, dynamic func, dynamic[] objects) {
            if (string.IsNullOrEmpty(methodId)) {
                throw new ArgumentException(nameof(methodId));
            }
            
            if (peer == null) {
                throw new ArgumentNullException(nameof(peer));
            }

            ResponseCallback responseCallback = (rm, sd) => {
                func?.Invoke(rm, sd);
            };
            Ssci.Invoke(methodId, peer, responseCallback, PacketShell(objects));
        }

        private object[] PacketShell(dynamic[] objects) {
            for (var i = 0; i < objects.Length; i++) {
                if (objects[i] is OldInstance) {
                    objects[i] = objects[i].ISerializablePacket.Value;
                }

                if (objects[i] is Array array) {
                    var type = objects[i].GetType().GetElementType();
                    if (!typeof(IConvertible).IsAssignableFrom(type)) {
                        var packets = typeof(ISerializablePacket).MakeArray(array.Length);
                        for (var j = 0; j < array.Length; j++) {
                            packets.SetValue((array.GetValue(j) as dynamic).ISerializablePacket.Value,j);
                        }
                        objects[i] = packets;
                    }
                    continue;
                }

                if (objects[i] is IDictionary dict) {
                    var arguments = dict.GetType().GetGenericArguments();
                    if (typeof(OldInstance).IsAssignableFrom(arguments[0])) {
                        arguments[0] = typeof(IPythonPacket);
                    }

                    if (typeof(OldInstance).IsAssignableFrom(arguments[1])) {
                        arguments[1] = typeof(IPythonPacket);
                    }

                    var newDict = DictionaryExtension.MakeDictionary(arguments[0], arguments[1]);
                    foreach (var item in dict.Keys) {
                        var value = dict[item];
                        object key = item;
                        if (arguments[0] == typeof(IPythonPacket)) {
                            dict.Remove(item);
                            key = (item as dynamic).ISerializablePacket.Value;
                        }

                        if (arguments[1] == typeof(IPythonPacket)) {
                            value = (value as dynamic).ISerializablePacket.Value;
                        }
                        newDict.Add(key, value);
                    }
                    objects[i] = newDict;
                    continue;
                }
            }
            return objects;
        }

        public string Register(string id, dynamic func, PythonHelper pythonHelper) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }

            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            object LateBoundMethod(params object[] args) {
                var objs = PythonParser.Parse(args);
                return pythonHelper.Call(func, objs);
            }

            RpcRegister.RegisterMethod(id, LateBoundMethod);
            return id;
        }
    }
}
