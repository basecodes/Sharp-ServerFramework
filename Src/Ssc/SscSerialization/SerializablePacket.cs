using Ssc.SscTemplate;

namespace Ssc.SscSerialization {

    public interface IObjectType {
        string TypeName { get; }
    }

    public abstract class SerializablePacket<T> : PoolAllocator<T>,IObjectType
         where T : ISerializablePacket {

        public virtual string TypeName => typeof(T).Name;
    }

    public abstract class SerializablePacket : ISerializablePacket {

        public virtual string TypeName => GetType().Name;

        public abstract void ToBinaryWriter(IEndianBinaryWriter writer);
        public abstract void FromBinaryReader(IEndianBinaryReader reader);

        public virtual void Assign() {
        }

        public virtual void Recycle() {
        }
    }
}