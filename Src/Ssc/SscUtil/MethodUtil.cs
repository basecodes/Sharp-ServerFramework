using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Ssc.SscUtil {

    public class methodof<T> {
        private MethodInfo method;

        public methodof(T func) {
            Delegate del = (Delegate)(object)func;
            this.method = del.Method;
        }


        public static implicit operator methodof<T>(T methodof) {
            return new methodof<T>(methodof);
        }

        public static implicit operator MethodInfo(methodof<T> methodof) {
            return methodof.method;
        }
    }
    public static class MethodUtil {

        public static MethodInfo GetMethodInfo<T>(Expression<T> expression) {
            var member = expression.Body as MethodCallExpression;

            if (member != null) {
                return member.Method;
            }

            throw new ArgumentException("Expression is not a method", "expression");
        }

        public static string MethodName(LambdaExpression expression) {
            var unaryExpression = (UnaryExpression)expression.Body;
            var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
            var methodCallObject = (ConstantExpression)methodCallExpression.Object;
            var methodInfo = (MethodInfo)methodCallObject.Value;

            return methodInfo.Name;
        }

        public static void SpendTime(Action action,long maxTime,Action<long> fail = null) {
            var stopwatch = Stopwatch.StartNew();
            action?.Invoke();
            stopwatch.Stop();
            fail?.Invoke(stopwatch.ElapsedMilliseconds);
        }
    }
}
