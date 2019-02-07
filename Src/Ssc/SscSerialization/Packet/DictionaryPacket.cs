using System;
using System.Collections.Generic;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Ssc.SscSerialization.Packet {
    public class DictionaryPacket<K, V> : ISerializablePacket
        where K : IConvertible
        where V : IConvertible {
        public Dictionary<K, V> value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryBB<K, V>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryBB(value);
        }

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }

    public class DictionaryPacket2<K, V> : ISerializablePacket
        where K : IConvertible
        where V : class ,ISerializablePacket{
        public Dictionary<K, V> value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryBP<K, V>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryBP(value);
        }

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }

    public class DictionaryPacket3<K, V> : ISerializablePacket
        where K : class,ISerializablePacket
        where V : class,ISerializablePacket {
        public Dictionary<K, V> value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryPP<K, V>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryPP(value);
        }

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }

    public class DictionaryPacket4<K, V> : ISerializablePacket
        where K : class,ISerializablePacket
        where V : IConvertible {
        public Dictionary<K, V> value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryPB<K, V>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryPB(value);
        }


        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }
}