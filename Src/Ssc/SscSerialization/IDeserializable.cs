namespace Ssc.SscSerialization {
    public interface IDeserializable {
        object[] Deserialize(object[] objects, int length);
    }
}