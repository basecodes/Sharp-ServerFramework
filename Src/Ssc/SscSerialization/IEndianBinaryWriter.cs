using System;
using System.Collections.Generic;
using Ssc.SscStream;

namespace Ssc.SscSerialization {
    public interface IEndianBinaryWriter {
        void Write<T>(T value) where T : IConvertible;
        void Write(TypeCode typeCode, IConvertible value);
        void WritePacket<T>(T packet) where T : class,ISerializablePacket;
        void WritePacket(string interfaceName, ISerializablePacket packet);
        void WriteArray<T>(T[] array) where T : IConvertible;
        void WriteArray(TypeCode typeCode, IConvertible[] array);
        void WritePacketArray<T>(T[] packets) where T : class,ISerializablePacket;
        void WritePacketArray(string interfaceName, ISerializablePacket[] packets);

        void WriteDictionaryBB<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : IConvertible;

        void WriteDictionaryBB(TypeCode keyTypeCode, TypeCode valueTypeCode,
            Dictionary<IConvertible, IConvertible> dictionary);

        void WriteDictionaryBP<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : class,ISerializablePacket;

        void WriteDictionaryBP(TypeCode keyTypeCode, string interfaceName,
            Dictionary<IConvertible, ISerializablePacket> dictionary);

        void WriteDictionaryPP<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : class,ISerializablePacket;

        void WriteDictionaryPP(string keyInterfaceName, string valueInterfaceName,
            Dictionary<ISerializablePacket, ISerializablePacket> dictionary);

        void WriteDictionaryPB<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : IConvertible;

        void WriteDictionaryPB(string interfaceName, TypeCode valueTypeCode,
            Dictionary<ISerializablePacket, IConvertible> dictionary);

        ByteFragment ToBytes();
    }
}