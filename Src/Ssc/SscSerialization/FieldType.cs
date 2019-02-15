namespace Ssc.SscSerialization {
    public enum FieldType : byte {
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