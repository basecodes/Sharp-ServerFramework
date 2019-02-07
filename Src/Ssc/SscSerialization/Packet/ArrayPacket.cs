using System;
using Ssc.SscStream;

namespace Ssc.SscSerialization.Packet {
    public class SimpleArrayPacket<T> : ISerializablePacket
        where T : IConvertible {
        public T[] value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadArray<T>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WriteArray(value);
        }

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }

    public class ArrayPacket<T> : ISerializablePacket
        where T : class,ISerializablePacket {
        public T[] value { get; private set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            value = reader.ReadPacketArray<T>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.WritePacketArray(value);
        }

        public void ToBytes(IWriteStream writeStream) {
            var endianBinaryWriter = new EndianBinaryWriter(writeStream);
            ToBinaryWriter(endianBinaryWriter);
        }
    }
}