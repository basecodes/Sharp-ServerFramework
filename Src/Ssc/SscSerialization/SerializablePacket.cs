using Ssc.Ssc;
using Ssc.SscTemplate;
using System;

namespace Ssc.SscSerialization {

    public interface IType {
        string TypeName { get; }
    }

    public abstract class SerializablePacket<T> : PoolAllocator<T>,IType
         where T : ISerializablePacket, IRecyclable, IAssignable {

        public virtual string TypeName => typeof(T).Name;
    }

    public abstract class SerializablePacket : ISerializablePacket {

        public virtual string TypeName => GetType().Name;

        public abstract void ToBinaryWriter(IEndianBinaryWriter writer);
        public abstract void FromBinaryReader(IEndianBinaryReader reader);
    }
}