namespace Ssc.SscSerialization {
    internal enum FieldType : byte {
        NullType,
        BaseType,
        PacketType,
        ArrayBase,
        ArrayPacket,
        DictKBVB,
        DictKBVP,
        DictKPVP,
        DictKPVB
    }
}