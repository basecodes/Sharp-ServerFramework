using System;
using System.Collections.Generic;
using Ssc.SscStream;

namespace Ssc.SscSerialization {
    public interface ISerializable : IDisposable {
        IWriteStream WriteStream { get; }
        
        ISerializable SerializeNull();
        ISerializable Serialize<T>(T value) where T : IConvertible;
        ISerializable Serialize(TypeCode typeCode, IConvertible value);
        ISerializable SerializePacket<T>(T packet) where T : class,ISerializablePacket;
        ISerializable SerializePacket(string interfaceName, ISerializablePacket packet);
        ISerializable SerializeArray<T>(T[] array) where T : IConvertible;
        ISerializable SerializeArray(TypeCode typeCode, IConvertible[] array);
        ISerializable SerializePacketArray<T>(T[] array) where T : class,ISerializablePacket;
        ISerializable SerializePacketArray(string interfaceName, ISerializablePacket[] array);

        ISerializable SerializeDictionaryBB<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : IConvertible;

        ISerializable SerializeDictionaryBB(TypeCode keyType, TypeCode valueType,
            Dictionary<IConvertible, IConvertible> dictionary);

        ISerializable SerializeDictionaryBP<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : class,ISerializablePacket;

        ISerializable SerializeDictionaryBP(TypeCode keyType, string interfaceName,
            Dictionary<IConvertible, ISerializablePacket> dictionary);

        ISerializable SerializeDictionaryPP<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : class,ISerializablePacket;

        ISerializable SerializeDictionaryPP(string keyInterfaceName, string valueInterfaceName,
            Dictionary<ISerializablePacket, ISerializablePacket> dictionary);

        ISerializable SerializeDictionaryPB<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : IConvertible;

        ISerializable SerializeDictionaryPB(string interfaceName, TypeCode keyType,
            Dictionary<ISerializablePacket, IConvertible> dictionary);

        void SerializableObject(object obj);
    }
}