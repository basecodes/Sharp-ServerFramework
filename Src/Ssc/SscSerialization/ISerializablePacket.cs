namespace Ssc.SscSerialization {
    public interface ISerializablePacket {
        
        void ToBinaryWriter(IEndianBinaryWriter writer);
        void FromBinaryReader(IEndianBinaryReader reader);
    }
}