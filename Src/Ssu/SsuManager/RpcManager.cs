using System;
using System.Collections.Generic;
using Ssc.SscLog;
using Ssc.SscRpc;

namespace Ssu.SsuManager {
    internal class RpcManager {
        private static readonly Dictionary<string, Func<object[],object>> _rpcMethods;
        private static readonly Logger Logger = LogManager.GetLogger<RpcManager>(LogType.Middle);
        public static NotHandler NotHandler = (id, args) => false;
        static RpcManager() {
            _rpcMethods = new Dictionary<string, Func<object[],object>>();
        }

        public static void AddRpcMethod(string methodId, Func<object[],object> method) {
            if (!_rpcMethods.ContainsKey(methodId)) {
                _rpcMethods.Add(methodId,method);
            } else {
                Logger.Warn($"{methodId} Rpc方法添加失败!");
            }
        }

        public static Func<object[],object> GetRpcMethod(string methodId) {
            _rpcMethods.TryGetValue(methodId, out var method);
            return method;
        }

        public static void AddNotHandlerMethod(NotHandler notHandled) {
            NotHandler = notHandled;
        }

        public static void RemoveRpcMethod(string methodId) {
            if (_rpcMethods.Remove(methodId)) {
                Logger.Warn($"{methodId} Rpc方法移除失败!");
            }
        }
    }
}