using System.Buffers;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ssc.SscLog;

namespace Ssc.SscFactory {
    public class ObjectFactory {
        private static readonly Dictionary<ConstructorInfo, object> _cache;

        private static readonly ArrayPool<byte> _arrayPool;
        static ObjectFactory() {
            _cache = new Dictionary<ConstructorInfo, object>();
            _arrayPool = ArrayPool<byte>.Create(
                Ssci.StreamConfig.BufferSize,
                Ssci.StreamConfig.BufferLength);
        }

        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor) {
            if (_cache.TryGetValue(ctor, out var obj)) {
                return obj as ObjectActivator<T>;
            }

            var type = ctor.DeclaringType;
            var paramsInfo = ctor.GetParameters();
            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];
            for (var i = 0; i < paramsInfo.Length; i++) {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;
                var paramAccessorExp = Expression.ArrayIndex(param, index);
                var paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            var newExp = Expression.New(ctor, argsExp);
            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            var compiled = (ObjectActivator<T>) lambda.Compile();
            _cache.Add(ctor, compiled);
            return compiled;
        }

        public static Logger Logger<T>() {
            return LogManager.GetLogger<T>(LogType.High);
        }

        public static Logger LowLogger<T>() {
            return LogManager.GetLogger<T>(LogType.Low);
        }

        public static Logger MiddleLogger<T>() {
            return LogManager.GetLogger<T>(LogType.Middle);
        }

        public static Logger Logger<T>(LogLevel defaulLogLevel, LogType logType) {
            var logger = LogManager.GetLogger<T>(logType);
            logger.LogLevel = defaulLogLevel;
            return logger;
        }

        public static byte[] CreateBuffer() {
            return _arrayPool.Rent(Ssci.StreamConfig.BufferSize);
        }

        public static void Recycle(byte[] bytes) {
            _arrayPool.Return(bytes);
        }
    }
}