using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ssc.SscFactory {
    /// <summary>
    ///     This class creates a generic method delegate from a MethodInfo signature
    ///     converting the method call into a LateBoundMethod delegate call. Using
    ///     this class allows making repeated calls very quickly.
    ///     Note: this class will be very inefficient for individual dynamic method
    ///     calls - compilation of the expression is very expensive up front, so using
    ///     this delegate factory makes sense only if you re-use and cache the dynamicly
    ///     loaded method repeatedly.
    ///     Entirely based on Nate Kohari's blog post:
    ///     http://kohari.org/2009/03/06/fast-late-bound-invocation-with-expression-trees/
    /// </summary>
    public static class DelegateFactory {
        public static T CreateReturnValueMethod<T>(object obj, MethodInfo method) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var instance = Expression.Convert(Expression.Constant(obj), obj.GetType());
            var call = Expression.Call(instance, method, CreateParameterExpressions(method, argumentsParameter));

            var body = Expression.Convert(call, typeof(object));
            var lambda = Expression.Lambda<T>(body, argumentsParameter);

            return lambda.Compile();
        }

        public static T CreateReturnVoidMethod<T>(object obj, MethodInfo method) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var instance = Expression.Convert(Expression.Constant(obj), obj.GetType());
            var call = Expression.Call(instance, method, CreateParameterExpressions(method, argumentsParameter));

            var lambda = Expression.Lambda<T>(call, argumentsParameter);

            return lambda.Compile();
        }

        public static T Create<T>(ConstructorInfo constructor, MethodInfo method) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var type = constructor.DeclaringType;
            var paramsInfo = constructor.GetParameters();
            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];
            for (var i = 0; i < paramsInfo.Length; i++) {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;
                var paramAccessorExp = Expression.ArrayIndex(param, index);
                argsExp[i] = Expression.Convert(paramAccessorExp, paramType);
            }

            var instance = Expression.New(constructor, argsExp);
            var call = Expression.Call(instance, method, CreateParameterExpressions(method, argumentsParameter));

            var lambda = Expression.Lambda<T>(
                Expression.Convert(call, typeof(object)),
                argumentsParameter);

            return lambda.Compile();
        }

        public static Func<object[],object> CreateValueDelegate<T>(Expression<T> expression,
            MethodInfo methodInfo) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var call = Expression.Invoke(
                expression,
                CreateParameterExpressions(methodInfo, argumentsParameter));

            var lambda = Expression.Lambda<Func<object[],object>>(
                Expression.Convert(call, typeof(object)),argumentsParameter);

            return lambda.Compile();
        }

        public static Action<object[]> CreateVoidDelegate<T>(Expression<T> expression,
            MethodInfo methodInfo) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var call = Expression.Invoke(
                expression,
                CreateParameterExpressions(methodInfo, argumentsParameter));

            var lambda = Expression.Lambda<Action<object[]>>(call,argumentsParameter);

            return lambda.Compile();
        }

        /// <summary>
        ///     Creates a LateBoundMethod delegate from a MethodInfo structure
        ///     Basically creates a dynamic delegate on the fly.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static T CreateStaticValueMethod<T>(MethodInfo method) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var call = Expression.Call(
                //Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            var lambda = Expression.Lambda<T>(Expression.Convert(call, typeof(object)),
                argumentsParameter);

            return lambda.Compile();
        }

        public static T CreateStaticVoidMethod<T>(MethodInfo method) {
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var call = Expression.Call(
                //Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            var lambda = Expression.Lambda<T>(call,
                argumentsParameter);

            return lambda.Compile();
        }

        /// <summary>
        ///     Creates a LateBoundMethod from type methodname and parameter signature that
        ///     is turned into a MethodInfo structure and then parsed into a dynamic delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static T CreateStaticValueMethod<T>(Type type, string methodName, params Type[] parameterTypes) {
            return CreateStaticValueMethod<T>(type.GetMethod(methodName, parameterTypes));
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter) {
            return method.GetParameters().Select((parameter, index) => {
                return Expression.Convert(Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)
                ), parameter.ParameterType);
            }).ToArray();
        }
    }

    /// <summary>
    ///     Collection of Reflection and type conversion related utility functions
    /// </summary>
    public class ReflectionUtils {
        /// <summary>
        ///     Binding Flags constant to be reused for all Reflection access methods.
        /// </summary>
        public const BindingFlags MemberAccess =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static object CallMethod(object instance, string method, Type[] parameterTypes, params object[] parms) {
            if (parameterTypes == null && parms.Length > 0)
                // Call without explicit parameter types - might cause problems with overloads
                // occurs when null parameters were passed and we couldn't figure out the parm type
                return instance.GetType().GetMethod(method, MemberAccess | BindingFlags.InvokeMethod)
                    .Invoke(instance, parms);
            return instance.GetType()
                .GetMethod(method, MemberAccess | BindingFlags.InvokeMethod, null, parameterTypes, null)
                .Invoke(instance, parms);
        }

        public static object CallMethod(object instance, string method, params object[] parms) {
            // Pick up parameter types so we can match the method properly
            Type[] parameterTypes = null;
            if (parms != null) {
                parameterTypes = new Type[parms.Length];
                for (var x = 0; x < parms.Length; x++) {
                    // if we have null parameters we can't determine parameter types - exit
                    if (parms[x] == null) {
                        parameterTypes = null; // clear out - don't use types
                        break;
                    }

                    parameterTypes[x] = parms[x].GetType();
                }
            }

            return CallMethod(instance, method, parameterTypes, parms);
        }
    }
}