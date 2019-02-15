namespace Ssc.SscSerialization {
    public interface ISerializablePacket:IType {
        
        void ToBinaryWriter(IEndianBinaryWriter writer);
        void FromBinaryReader(IEndianBinaryReader reader);
    }
}