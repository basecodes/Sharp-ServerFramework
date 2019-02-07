using System;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Ssc.SscSerialization.Packet {
    public class SimplePacket<T> : ISerializablePacket
        where T : IConvertible {
        public T value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.Read<T>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.Write(value);
        }

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }
}