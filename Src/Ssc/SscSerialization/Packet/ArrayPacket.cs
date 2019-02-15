using System;
using Ssc.SscStream;

namespace Ssc.SscSerialization.Packet {
    public class SimpleArrayPacket<T> : SerializablePacket
        where T : IConvertible {
        public T[] value { get; private set; }
        public override string TypeName => typeof(T[]).Name;
        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadArray<T>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteArray(value);
        }
    }

    public class ArrayPacket<T> : SerializablePacket
        where T : class,ISerializablePacket {
        public T[] value { get; private set; }
        public override string TypeName => typeof(T[]).Name;
        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadPacketArray<T>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WritePacketArray(value);
        }
    }
}