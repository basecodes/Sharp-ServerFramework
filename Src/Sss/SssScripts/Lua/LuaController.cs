using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Ssc;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscRpc;

namespace Sss.SssScripts.Lua {
    internal sealed class LuaController : Controller {
        private static readonly Logger Logger = LogManager.GetLogger<LuaController>(LogType.Middle);

        public Table Instance { get; }

        public LuaController(Table table) {
            Instance = table ?? throw new ArgumentNullException(nameof(table));
        }

        public string Register(string id, Closure func) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }

            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            object LateBoundMethod(params object[] args) {
                var list = new List<object> { Instance };
                var objs = LuaParser.Parse(args,Instance.OwnerScript);
                list.AddRange(objs);

                return func.Call(list.ToArray()).ToObject();
            }

            RpcRegister.RegisterMethod(id, LateBoundMethod);
            return id;
        }

        public void Invoke(string methodId, IPeer peer, Closure closure, params object[] objects) {
            if (string.IsNullOrEmpty(methodId)) {
                throw new ArgumentException(nameof(methodId));
            }

            if (peer == null) {
                throw new ArgumentNullException(nameof(peer));
            }

            ResponseCallback responseCallback = (rm, sd) => {
                closure?.Call(rm, sd);
            };

            Invoke(methodId, peer, responseCallback, objects);
        }

        public void Invoke(string methodId, IPeer peer, ResponseCallback responseCallback, params object[] objects) {
            Ssci.Invoke(methodId, peer, responseCallback, (serializable) => {
                foreach (var item in objects) {
                    serializable.SerializableObject(item);
                }
            });
        }
    }
}