using System;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Ssc.SscSerialization.Packet {
    public class SimplePacket<T> : SerializablePacket
        where T : IConvertible {

        public override string TypeName => typeof(T).Name;
        public T value { get; private set; }

        public override void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.Read<T>();
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.Write(value);
        }
    }
}