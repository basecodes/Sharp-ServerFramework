using IronPython.Runtime;
using Ssc;
using Ssc.Ssc;
using Ssc.SscRpc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sss.SssScripts.Pythons {
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

        public static string Register(string key, dynamic func,PythonHelper pythonHelper) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException(nameof(key));
            }

            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            var pattern = @"^[+][\[](.*)[\]][+]$";
            var match = Regex.Match(key, pattern);
            if (!match.Success) {
                throw new ArgumentException(nameof(key));
            }

            object LateBoundMethod(params object[] args) {
                return pythonHelper.Call(func, args);
            }

            var id = match.Groups[1].Value;
            RpcRegister.RegisterMethod(id, LateBoundMethod);
            return id;
        }
    }
}
