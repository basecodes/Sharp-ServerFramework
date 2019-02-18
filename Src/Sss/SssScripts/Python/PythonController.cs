using IronPython.Runtime;
using IronPython.Runtime.Types;
using Ssc;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscRpc;
using Sss.SssSerialization.Python;
using System;
using System.Linq;
using System.Collections;

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
                if (objects[i] is Array array) {
                    var type = objects[i].GetType().GetElementType();
                    if (typeof(IPythonObject).IsAssignableFrom(type)) {
                        for (var j = 0; j < array.Length; j++) {
                            array.SetValue(j, (array.GetValue(j) as dynamic).ISerializablePacket.Value);
                        }
                    }
                }

                if (objects[i] is IDictionary dict) {
                    var arguments = objects[i].GetGenericArguments();
                    if (typeof(IPythonObject).IsAssignableFrom(arguments[0])) {
                        foreach (var item in dict.Keys) {
                            var value = dict[item];
                            dict.Remove(item);
                            dict[(item as dynamic).ISerializablePacket.Value] = value;
                        }
                    }

                    if (typeof(IPythonObject).IsAssignableFrom(arguments[1])) {
                        foreach (var item in dict.Keys) {
                            var value = dict[item];
                            dict[item] = (value as dynamic).ISerializablePacket.Value;
                        }
                    }
                }

                if (!(objects[i] is IConvertible)) {
                    objects[i] = objects[i].ISerializablePacket.Value;
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
