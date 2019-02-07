using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ssc.SscAttribute;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscUtil;

namespace Ssc.SscRpc {

    public sealed class RpcRegister {
        private static readonly Logger Logger = LogManager.GetLogger<RpcRegister>(LogType.Middle);

        static RpcRegister() {
        }

        /// <summary>
        /// 注册无返回值RPC方法
        /// </summary>

        public static void RegisterVoidMethod(object obj, MethodInfo methodInfo) {

            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            
            var dge = DelegateFactory.CreateReturnVoidMethod<Action<object[]>>(obj, methodInfo);
            Func<object[], object> returnValueMethod = (args) => {
                dge.Invoke(args);
                return null;
            };

            RegisterMethod(methodInfo, returnValueMethod);
        }
        
        public static Func<object[], object> RegisterVoidMethod(string id,object obj, MethodInfo methodInfo) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }
            
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var dge = DelegateFactory.CreateReturnVoidMethod<Action<object[]>>(obj, methodInfo);
            Func<object[], object> returnValueMethod = (args) => {
                dge.Invoke(args);
                return null;
            };

            RegisterMethod(id,returnValueMethod);
            return returnValueMethod;
        }

        public static void RegisterMethod(MethodInfo method,Func<object[], object> callback) {

            var rpcResponse = method.GetCustomAttribute<RpcResponseAttribute>(true);
            var id = rpcResponse.GetId();

            if (RpcResponseManager.GetRpcMethod(id)  != null) {
                Logger.Warn($"方法：{id}已经注册！");
                return;
            }

            RpcResponseManager.AddRpcMethod(id, callback);
        }
        
        public static void RegisterMethod(string id,Func<object[], object> callback) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentNullException(nameof(id));
            }
            
            if (RpcResponseManager.GetRpcMethod(id)  != null) {
                Logger.Warn($"方法：{id}已经注册！");
                return;
            }

            RpcResponseManager.AddRpcMethod(id, callback);
        }

        public static void RegisterStaticVoidMethod(MethodInfo methodInfo) {

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var dge = DelegateFactory.CreateStaticVoidMethod<Action<object[]>>(methodInfo);
            Func<object[],object> returnValueMethod = (args) => {
                dge.Invoke(args);
                return null;
            };

            RegisterMethod(methodInfo, returnValueMethod);
        }
        
        public static Func<object[],object> RegisterStaticVoidMethod(string id,MethodInfo methodInfo) {

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var dge = DelegateFactory.CreateStaticVoidMethod<Action<object[]>>(methodInfo);
            Func<object[],object> returnValueMethod = (args) => {
                dge.Invoke(args);
                return null;
            };

            RegisterMethod(id,returnValueMethod);
            return returnValueMethod;
        }

        /// <summary>
        /// 注册有返回值RPC方法
        /// </summary>

        public static void RegisterValueMethod(object obj, MethodInfo methodInfo) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var returnValueMethod = DelegateFactory.CreateReturnValueMethod<Func<object[],object>>(obj, methodInfo);

            RegisterMethod(methodInfo, returnValueMethod);
        }
        
        public static Func<object[], object> RegisterValueMethod(string id,object obj, MethodInfo methodInfo) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }
            
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var returnValueMethod = DelegateFactory.CreateReturnValueMethod<Func<object[],object>>(obj, methodInfo);

            RegisterMethod(id, returnValueMethod);
            return returnValueMethod;
        }
        
        public static void RegisterStaticValueMethod(MethodInfo methodInfo) {
            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var returnValueMethod = DelegateFactory.CreateStaticValueMethod<Func<object[],object>>(methodInfo);
            RegisterMethod(methodInfo, returnValueMethod);
            
        }
        
        public static Func<object[], object> RegisterStaticValueMethod(string id,MethodInfo methodInfo) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }

            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var returnValueMethod = DelegateFactory.CreateStaticValueMethod<Func<object[],object>>(methodInfo);
            RegisterMethod(id, returnValueMethod);
            return returnValueMethod;
        }

        /// <summary>
        /// 注册Delegate
        /// </summary>
        public static void RegisterDelegate<T>(Expression<T> expression)where T:Delegate {
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }

            var methodInfo = MethodUtil.GetMethodInfo(expression);

            Func<object[], object> func;
            if (methodInfo.ReturnType != typeof(void)) {
                func = DelegateFactory.CreateValueDelegate(expression, methodInfo);
            } else {
                var dge = DelegateFactory.CreateVoidDelegate(expression, methodInfo);
                func = (args) => {
                    dge.Invoke(args);
                    return null;
                };
            }

            RegisterMethod(methodInfo, func);
        }
        
        public static Func<object[], object> RegisterDelegate<T>(string id,Expression<T> expression)where T:Delegate {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException(nameof(id));
            }
            
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }

            var methodInfo = MethodUtil.GetMethodInfo(expression);

            Func<object[], object> func;
            if (methodInfo.ReturnType != typeof(void)) {
                func = DelegateFactory.CreateValueDelegate(expression, methodInfo);
            } else {
                var dge = DelegateFactory.CreateVoidDelegate(expression, methodInfo);
                func = (args) => {
                    dge.Invoke(args);
                    return null;
                };
            }

            RegisterMethod(id, func);
            return func;
        }

        /// <summary>
        /// RPC对象注册
        /// </summary>
        public static List<string> RegisterMethod<T>(T rpcObject){
            if (rpcObject == null) {
                throw new ArgumentNullException(nameof(rpcObject));
            }

            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<RpcResponseAttribute>(true) != null);

            var rpcMethods = new List<string>();
            foreach (var methodInfo in methods) {
                if (methodInfo.ReturnType != typeof(void)) {
                    RegisterValueMethod(rpcObject, methodInfo);
                } else {
                    RegisterVoidMethod(rpcObject, methodInfo);
                }
                var rpcResponse = methodInfo.GetCustomAttribute<RpcResponseAttribute>(true);
                var id = rpcResponse.GetId();
                rpcMethods.Add(id);
            }

            return rpcMethods;
        }

        /// <summary>
        /// 从对象里注册RPC方法
        /// </summary>
        public static List<string> RegisterMethod(Type type,object obj) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<RpcResponseAttribute>(true) != null);

            var rpcMethods = new List<string>();
            foreach (var methodInfo in methods) {

                if (methodInfo.ReturnType != typeof(void)) {
                    RegisterValueMethod(obj, methodInfo);
                } else {
                    RegisterVoidMethod(obj, methodInfo);
                }

                var rpcResponse = methodInfo.GetCustomAttribute<RpcResponseAttribute>(true);
                var id = rpcResponse.GetId();
                rpcMethods.Add(id);
            }

            return rpcMethods;
        }

        public static void RemoveMethod(string methodId) {
            RpcResponseManager.RemoveRpcMethod(methodId);
        }
    }
}