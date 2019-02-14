using Ssc.Ssc;
using Ssc.SscConfiguration;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscSerialization;
using Ssc.SscUtil;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ssc {

    public class Method {
        public string Id { get; internal set; }
        public Func<object[],object> Function { get; internal set; }
    }

    public class Ssci {
        private static readonly Logger Logger = LogManager.GetLogger<Ssci>(LogType.Middle);
        public static StreamConfig StreamConfig { get; }
        static Ssci() {
            StreamConfig = new StreamConfig();
        }

        public static void Initialize() {
        }

        public static void Invoke(string methodId, IPeer peer, Action<ISerializable> action) {
            Invoke(methodId, peer, null, action);
        }

        public static void Invoke(string methodId, IPeer peer, ResponseCallback responseCallback, Action<ISerializable> action) {
            using (var serializable = new Serializable(null)) {
                action?.Invoke(serializable);
                var rpcInvoke = new RpcInvoke("", methodId);
                RpcProxy.Invoke(MessageType.RpcInvoke, rpcInvoke, serializable.WriteStream, peer, responseCallback);
            }
        }

        private static void Invoke<T>(Func<Expression<T>> func, IPeer peer, ResponseCallback responseCallback) {
            
            var methodTuple = RpcRequestManager.Get(func);
            var method = func?.Invoke().Body as MethodCallExpression;
            var array = new object[method.Arguments.Count];

            for (var i = 0; i < array.Length; i++) {
                var arg = method.Arguments;
                if (arg[i] is ConstantExpression v1) {
                    array[i] = v1.Value;
                }

                if (arg[i] is MemberExpression v2) {
                    var container = ((ConstantExpression)v2.Expression).Value;
                    var propertyInfo = v2.Member as FieldInfo;
                    array[i] = propertyInfo.GetValue(container);
                }
            }

            methodTuple.Action.Invoke(methodTuple.RpcMethod.GetId(), peer, responseCallback, array);
        }
        public static void Invoke<T>(Func<Expression<Action<T>>> func, IPeer peer, ResponseCallback responseCallback) {
            Invoke<Action<T>>(func, peer, responseCallback);
        }
        public static void Invoke<T>(Func<Expression<Action<T>>> func, IPeer peer) {
            Invoke<Action<T>>(func, peer, null);
        }

        public static void Invoke(string methodId,IPeer peer,ResponseCallback responseCallback, params object[] objects) {
            Invoke(methodId, peer, responseCallback, (serializable) => {
                foreach (var item in objects) {
                    serializable.SerializableObject(item);
                }
            });
        }

        public static void Invoke(string methodId, IPeer peer,params object[] objects) {
            Invoke(methodId, peer, null, objects);
        }

        public static void Notify<T>(T statusCode, string message, IPeer peer) where T : struct, Enum {
            Notify(statusCode.ToString(), message, peer);
        }
        public static void Notify(string statusCode, string message, IPeer peer) {
            Invoke(statusCode, peer, (serializable) => {
                serializable.Serialize(statusCode.ToString());
                serializable.Serialize(message);
            });
        }

        private static object GetObject<T>(Expression<T> expression) {
            var methodCall = expression.Body as MethodCallExpression;
            var constant = methodCall.Object as ConstantExpression;
            if (constant.Value == null) {
                throw new ExpressionException("Left为null!");
            }
            return constant.Value;
        }

        public static Method Register<TDelegate>(
            string methodId,
            Expression<TDelegate> implementExpression) where TDelegate : Delegate {

            if (implementExpression == null) {
                throw new ArgumentNullException(nameof(implementExpression));
            }

            var method = MethodUtil.GetMethodInfo(implementExpression);

            if (!typeof(Delegate).IsAssignableFrom(method.DeclaringType)) {
                throw new ArgumentException($"{nameof(implementExpression)}不是delegate类型！");
            }

            var func = RpcRegister.RegisterDelegate(methodId, implementExpression);
            return new Method { Id = methodId, Function = func };
        }

        public static Method Register<TInterface, TDelegate>(
            Expression<Action<TInterface>> interfaceExpression,
            Expression<TDelegate> implementExpression)
            where TDelegate : Delegate {

            if (interfaceExpression == null) {
                throw new ArgumentNullException(nameof(interfaceExpression));
            }

            if (implementExpression == null) {
                throw new ArgumentNullException(nameof(implementExpression));
            }
            var id = RpcResponseManager.GetId(interfaceExpression);
            return Register(id, implementExpression);
        }

        public static Method Register<TInterface>(
            Expression<Action<TInterface>> interfaceExpression,
            Expression<Action> implementExpression) 
            where TInterface : class {

            if (interfaceExpression == null) {
                throw new ArgumentNullException(nameof(interfaceExpression));
            }

            if (implementExpression == null) {
                throw new ArgumentNullException(nameof(implementExpression));
            }
            var id = RpcResponseManager.GetId(interfaceExpression);
            return Register(id, implementExpression);
        }

        public static Method Register(
            string methodId,
            Expression<Action> implementExpression){

            if (implementExpression == null) {
                throw new ArgumentNullException(nameof(implementExpression));
            }

            var method = MethodUtil.GetMethodInfo(implementExpression);

            if (typeof(Delegate).IsAssignableFrom(method.DeclaringType)) {
                throw new ArgumentException($"{nameof(implementExpression)}不能为Delegate！");
            }

            Func<object[], object> func = null;

            if (method.IsStatic) {
                if (method.ReturnType == typeof(void)) {
                    func = RpcRegister.RegisterStaticVoidMethod(methodId, method);
                } else {
                    func = RpcRegister.RegisterStaticValueMethod(methodId, method);
                }
            } else {
                if (method.ReturnType == typeof(void)) {
                    func = RpcRegister.RegisterVoidMethod(methodId, GetObject(implementExpression), method);
                } else {
                    func = RpcRegister.RegisterValueMethod(methodId, GetObject(implementExpression), method);
                }
            }

            return new Method { Id = methodId, Function = func }; 
        }

        public static Method Register<T>(
            T value,
            Action<T, string> callback) where T : struct, Enum {

            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            var methodId = value.ToString();
            Action<string, string> statusCallback = (status, message) => {
                if (Enum.TryParse(status, out T enumValue)) {
                    callback?.Invoke(enumValue, message);
                } else {
                    Logger.Warn($"{status}转为{typeof(T).Name}类型失败！");
                }
            };

           return Register(value.ToString(), statusCallback);
        }

        public static Method Register(
            string methodId,
            Action<string, string> callback) {

            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }

            var method = callback.Method;
            if (typeof(Delegate).IsAssignableFrom(method.DeclaringType)) {
                if (method.ReturnType == typeof(void)) {
                    return Register<Action<string, string>>(methodId, (str1, str2) => callback.Invoke(str1, str2));
                }
            } else {
                return Register<Action<string, string>>(methodId, (arge0, arge1) => callback.Invoke(arge0, arge1));
            }
            return null;
        }
    }
}