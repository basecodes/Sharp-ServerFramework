using System;
using System.Collections.Generic;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Ssc.SscSerialization.Packet {
    public class DictionaryPacket<K, V> : SerializablePacket
        where K : IConvertible
        where V : IConvertible {
        public Dictionary<K, V> value { get; private set; }
        public override string TypeName => typeof(Dictionary<K, V>).Name;
        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryBB<K, V>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryBB(value);
        }
    }

    public class DictionaryPacket2<K, V> : SerializablePacket
        where K : IConvertible
        where V : class ,ISerializablePacket{
        public Dictionary<K, V> value { get; private set; }
        public override string TypeName => typeof(Dictionary<K, V>).Name;
        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryBP<K, V>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryBP(value);
        }
    }

    public class DictionaryPacket3<K, V> : SerializablePacket
        where K : class,ISerializablePacket
        where V : class,ISerializablePacket {
        public Dictionary<K, V> value { get; private set; }
        public override string TypeName => typeof(Dictionary<K, V>).Name;
        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryPP<K, V>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryPP(value);
        }
    }

    public class DictionaryPacket4<K, V> : SerializablePacket
        where K : class,ISerializablePacket
        where V : IConvertible {
        public Dictionary<K, V> value { get; private set; }
        public override string TypeName => typeof(Dictionary<K, V>).Name;

        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadDictionaryPB<K, V>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteDictionaryPB(value);
        }
    }
}