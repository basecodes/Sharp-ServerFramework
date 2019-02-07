using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ssc.SscAttribute;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscUtil;

namespace Ssc.SscRpc {
    public delegate bool NotHandler(string methoId,params object[] args);
    public class RpcResponseManager {

        private static readonly Dictionary<string, Func<object[], object>> _rpcMethods;
        private static readonly Logger Logger = LogManager.GetLogger<RpcResponseManager>(LogType.Middle);
        public static NotHandler NotHandler = (id, args) => false;
        static RpcResponseManager() {
            _rpcMethods = new Dictionary<string, Func<object[], object>>();
        }


        public static string GetId<T>(Expression<T> expression) {
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }

            var method = MethodUtil.GetMethodInfo(expression);
            var response = method.GetCustomAttribute<RpcResponseAttribute>();
            if (response == null) {
                throw new MissingAttributeException($"{method.Name}缺少{nameof(RpcResponseAttribute)}属性");
            }

            return response.GetId();
        }

        public static void AddRpcMethod(string methodId, Func<object[], object> method) {
            if(_rpcMethods.TryGetValue(methodId, out _)) {
                Logger.Warn($"{methodId} Rpc方法添加失败!");
                return;
            }
            _rpcMethods.Add(methodId, method);
        }

        public static Func<object[], object> GetRpcMethod(string methodId) {
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
