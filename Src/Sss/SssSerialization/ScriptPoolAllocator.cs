using System;
using System.Collections.Generic;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscPool;
using Ssc.SscSerialization;

namespace Sss.SssSerialization {
    public abstract class ScriptPoolAllocator<T>:SerializablePacket<T> 
        where T : ISerializablePacket {
        

        private  static readonly Dictionary<string, ObjectPool<T>> _objectPools = new Dictionary<string, ObjectPool<T>>();
        private static readonly Logger Logger = LogManager.GetLogger<ScriptPoolAllocator<T>>(LogType.Middle);

        public static void SetPool(string interfaceName, ObjectActivator<T> objectGenerator) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            var objectPool = new ObjectPool<T>(objectGenerator);
            if (!_objectPools.ContainsKey(interfaceName)) {
                _objectPools.Add(interfaceName, objectPool);   
            }
        }

        public static T GetObject(string interfaceName, params object[] args) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (!_objectPools.TryGetValue(interfaceName, out var objectPool)) {
                throw new UnregisteredException(interfaceName);
            }

            return objectPool.GetObject(args);
        }

        public static void Recycle(string interfaceName,T value) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (!_objectPools.TryGetValue(interfaceName, out var objectPool)) {
                throw new UnregisteredException(interfaceName);
            }
            objectPool.PutObject(value);
        }
    }
}