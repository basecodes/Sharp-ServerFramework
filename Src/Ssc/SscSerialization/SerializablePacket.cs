namespace Ssc.SscSerialization {
//    public abstract class SerializablePacket<T> :
//        PoolTemplate<T>,ISerializablePacket where T : IRecyclable,IAssignable {
//        
//        public abstract void ToBinaryWriter(IEndianBinaryWriter writer);
//        public abstract void FromBinaryReader(IEndianBinaryReader reader);
//    }
    
    public abstract class SerializablePacket :ISerializablePacket  {
        public abstract void ToBinaryWriter(IEndianBinaryWriter writer);
        public abstract void FromBinaryReader(IEndianBinaryReader reader);
    }
}