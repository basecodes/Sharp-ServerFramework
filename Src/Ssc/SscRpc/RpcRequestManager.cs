using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ssc.Ssc;
using Ssc.SscAttribute;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscUtil;

namespace Ssc.SscRpc {

    public class MethodTuple{
        public RpcMethodAttribute RpcMethod { get; set; }
        public Action<string,IPeer,ResponseCallback,object[]> Action { get; set; }
    }
    
    public class RpcRequestManager {

        private static readonly Logger Logger = LogManager.GetLogger<RpcRequestManager>(LogType.Middle);
        private static Dictionary<string, MethodTuple> _dict;

        static RpcRequestManager() {
            _dict = new Dictionary<string, MethodTuple>();
        }

        public static MethodTuple CreateMethodTuple<T>(Expression<T> expression) {
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }

            var method = MethodUtil.GetMethodInfo(expression);
            var response = method.GetCustomAttribute<RpcRequestAttribute>();
            if (response == null) {
                throw new MissingAttributeException($"{method.Name}缺少{nameof(RpcRequestAttribute)}属性");
            }

            var dge = CreateDelegate(expression);
            var methodTupe = new MethodTuple() {
                RpcMethod = response,
                Action = dge
            };
            
            return methodTupe;
        }

        public static MethodTuple CreateMethodTuple<T>(Func<Expression<T>> func) {
            var name = func.Method.Name;
            var methodTupe = Get(name);
            if (methodTupe == null) {
                methodTupe = CreateMethodTuple(func?.Invoke());
                Add(name, methodTupe);
            }
            return methodTupe;
        }

         private static Action<string,IPeer,ResponseCallback,object[]> CreateDelegate<T>(Expression<T> expression) {
            var method = expression.Body as MethodCallExpression;
            
            var methodExpression = Expression.Parameter(typeof(string), "methodId");
            var peerExpression = Expression.Parameter(typeof(IPeer), "peer");
            var responseCallbackExpression = Expression.Parameter(typeof(ResponseCallback), "methodId");
            var objectsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var list = new List<Expression>();
            var se = Expression.New(typeof(Serializable).GetConstructors().First(), Expression.Constant(null));
            var instace = Expression.Variable(typeof(Serializable));
            var serObj = Expression.Assign(instace, se);
            list.Add(serObj);

            var expressions = CreateExpressions(method.Method, instace, Expression.Convert(objectsParameter,typeof(object[])));
            list.AddRange(expressions);

            var ctor = typeof(RpcInvoke).GetConstructors().First();
            var rpcInvokeExpression = Expression.New(ctor,Expression.Constant(""),
                Expression.Convert(methodExpression,typeof(string)));

            var rpcInvoke = Expression.Call(typeof(RpcProxy).GetMethod(nameof(RpcProxy.Invoke),
                        new Type[] { typeof(MessageType), typeof(RpcInvoke), typeof(IWriteStream),typeof(IPeer), typeof(ResponseCallback) }),
                        Expression.Constant(MessageType.RpcInvoke), 
                        Expression.Convert(rpcInvokeExpression,typeof(IRpc)),
                        Expression.Property(instace,typeof(ISerializable).GetProperty(nameof(ISerializable.WriteStream))),
                        Expression.Convert(peerExpression,typeof(IPeer)),
                        Expression.Convert(responseCallbackExpression,typeof(ResponseCallback)));
            list.Add(rpcInvoke);

            var expressionsDestroy = Expression.Call(instace, typeof(Serializable).GetMethod("Dispose"));
            list.Add(expressionsDestroy);
            
            var blockExpression = Expression.Block(new[] { instace },list);

            var lambda = Expression.Lambda<Action<string,IPeer,ResponseCallback,object[]>>(blockExpression,
                methodExpression,peerExpression,responseCallbackExpression,objectsParameter);
            
            return lambda.Compile();
        }

        private static List<Expression> CreateExpressions(MethodInfo method,Expression expression,Expression argumentsParameter) {
            return method.GetParameters().Select((parameter, index) => {

                var arg = Expression.Convert(Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)), 
                    parameter.ParameterType);
                
                var methods = typeof(ISerializable).GetMethods();
                
                if (typeof(IConvertible).IsAssignableFrom(parameter.ParameterType)) {
                    var methodName = nameof(ISerializable.Serialize);
                    var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                    return Expression.Call(expression, methodInfo.MakeGenericMethod(parameter.ParameterType) ,arg);
                }

                if (typeof(ISerializablePacket).IsAssignableFrom(parameter.ParameterType)) {
                    var methodName = nameof(ISerializable.SerializePacket);
                    var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                    return Expression.Call(expression,methodInfo.MakeGenericMethod(parameter.ParameterType),arg);
                }

                if (typeof(Array).IsAssignableFrom(parameter.ParameterType)) {
                    var elementType = parameter.ParameterType.GetElementType();
                    
                    if (typeof(IConvertible).IsAssignableFrom(elementType)) {
                        var methodName = nameof(ISerializable.SerializeArray);
                        var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                        return Expression.Call(expression,methodInfo.MakeGenericMethod(elementType),arg);
                    } else {
                        var methodName = nameof(ISerializable.SerializePacketArray);
                        var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                        return Expression.Call(expression,methodInfo.MakeGenericMethod(elementType),arg);
                    }
                }

                if (typeof(IDictionary).IsAssignableFrom(parameter.ParameterType)) {
                    var types = parameter.ParameterType.GenericTypeArguments;
                    var keyType = types[0];
                    var valueType = types[1];
                    if (typeof(IConvertible).IsAssignableFrom(keyType) && typeof(IConvertible).IsAssignableFrom(valueType)) {
                        var methodName = nameof(ISerializable.SerializeDictionaryBB);
                        var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                        return Expression.Call(expression,methodInfo.MakeGenericMethod(keyType,valueType),arg);
                    }

                    if (typeof(IConvertible).IsAssignableFrom(keyType) && valueType.IsByRef) {
                        var methodName = nameof(ISerializable.SerializeDictionaryBP);
                        var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                        return Expression.Call(expression,methodInfo.MakeGenericMethod(keyType,valueType),arg);
                    }

                    if (keyType.IsByRef && valueType.IsByRef) {
                        var methodName = nameof(ISerializable.SerializeDictionaryPP);
                        var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                        return Expression.Call(expression,methodInfo.MakeGenericMethod(keyType,valueType),arg);
                    }

                    if (keyType.IsByRef && typeof(IConvertible).IsAssignableFrom(valueType)) {
                        var methodName = nameof(ISerializable.SerializeDictionaryPB);
                        var methodInfo = methods.First(m => m.IsGenericMethod && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
                        return Expression.Call(expression,methodInfo.MakeGenericMethod(keyType,valueType),arg);
                    }
                }

                throw new ArgumentException($"{parameter.ParameterType.Name}参数类型不支持！");
            }).ToList<Expression>();
        }
        
        public static void Add(string name,MethodTuple methodTuple) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("message", nameof(name));
            }

            if (methodTuple == null) {
                throw new ArgumentNullException(nameof(methodTuple));
            }

            if (_dict.ContainsKey(name)) {
                Logger.Warn("已添加Name:" + name);
                return;
            }

            _dict.Add(name, methodTuple);
        }

        public static MethodTuple Get(string name) {
            _dict.TryGetValue(name, out var result);
            return result;
        }

        public static void Remove(string name) {
            _dict.Remove(name);
        }
    }
}
