using Ssc.Ssc;

namespace Ssc.SscSerialization {
    public interface ISerializablePacket:IObjectType,IMemoryable {
        
        void ToBinaryWriter(IEndianBinaryWriter writer);
        void FromBinaryReader(IEndianBinaryReader reader);
    }
}