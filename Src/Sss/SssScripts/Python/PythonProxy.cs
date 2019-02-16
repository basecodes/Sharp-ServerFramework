using Ssc;
using Ssc.Ssc;
using Ssc.SscRpc;
using System;

namespace Sss.SssScripts.Python {
    public class PythonProxy {

        public static void Invoke(string methodId, IPeer peer, dynamic func, dynamic[] objects) {
            if (string.IsNullOrEmpty(methodId)) {
                throw new ArgumentException(nameof(methodId));
            }

            if (peer == null) {
                throw new ArgumentNullException(nameof(peer));
            }

            ResponseCallback responseCallback = (rm, sd) => {
                func?.Invoke(rm, sd);
            };
            Ssci.Invoke(methodId, peer, responseCallback, objects);
        }

        public static string Register(string id, dynamic func,PythonHelper pythonHelper) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }

            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            object LateBoundMethod(params object[] args) {
                return pythonHelper.Call(func, args);
            }

            RpcRegister.RegisterMethod(id, LateBoundMethod);
            return id;
        }
    }
}
