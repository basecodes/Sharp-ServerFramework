using System.Collections.Generic;
using Ssc.Ssc;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscPool;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Sss.SssSerialization {
    internal abstract class LuaSerializablePacket<T> : ISerializablePacket where T : class, IRecyclable {
        
        private  static readonly Dictionary<string, ObjectPool<T>> 
            _objectPools = new Dictionary<string, ObjectPool<T>>();
        private static readonly Logger Logger = LogManager.GetLogger<LuaSerializablePacket<T>>(LogType.Middle);

        public abstract void ToBinaryWriter(IEndianBinaryWriter writer);

        public abstract void FromBinaryReader(IEndianBinaryReader reader);

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }

        public static void CreateObjectPool(string interfaceName,
            ObjectActivator<T> objectGenerator) {
            var objectPool = new ObjectPool<T>(objectGenerator);
            if (!_objectPools.ContainsKey(interfaceName)) {
                _objectPools.Add(interfaceName, objectPool);   
            }
        }

        public static T GetObject(string interfaceName, params object[] args) {
            if (!_objectPools.TryGetValue(interfaceName, out var objectPool)) {
                throw new UnregisteredException(interfaceName);
            }

            return objectPool.GetObject(args);
        }

        public static void Recycle(string interfaceName,T value) {
            if (!_objectPools.TryGetValue(interfaceName, out var objectPool))
                throw new UnregisteredException(interfaceName);
            objectPool.PutObject(value);
        }
    }
}