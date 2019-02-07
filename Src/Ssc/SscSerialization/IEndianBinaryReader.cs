using System;
using System.Collections;
using System.Collections.Generic;

namespace Ssc.SscSerialization {
    public interface IEndianBinaryReader {
        T Read<T>() where T : IConvertible;
        IConvertible Read();
        T ReadPacket<T>() where T : class,ISerializablePacket;
        ISerializablePacket ReadPacket();
        T[] ReadArray<T>() where T : IConvertible;
        Array ReadArray();
        T[] ReadPacketArray<T>() where T : class,ISerializablePacket;
        Array ReadPacketArray();

        Dictionary<K, V> ReadDictionaryBB<K, V>()
            where K : IConvertible
            where V : IConvertible;

        IDictionary ReadDictionaryBB();

        Dictionary<K, V> ReadDictionaryBP<K, V>()
            where K : IConvertible
            where V : class,ISerializablePacket;

        IDictionary ReadDictionaryBP();

        Dictionary<K, V> ReadDictionaryPP<K, V>()
            where K : class,ISerializablePacket
            where V : class,ISerializablePacket;

        IDictionary ReadDictionaryPP();

        Dictionary<K, V> ReadDictionaryPB<K, V>()
            where K : class,ISerializablePacket
            where V : IConvertible;

        IDictionary ReadDictionaryPB();
    }
}