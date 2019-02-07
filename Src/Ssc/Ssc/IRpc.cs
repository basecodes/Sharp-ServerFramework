using Ssc.SscStream;

namespace Ssc.Ssc {
    public enum MessageType : short {
        None,
        RpcInvoke = -127,
        RpcResponse,
    }

    public interface IRpc {
        // 序列化字段
        void SerializeFields(IWriteStream writeStream);
        // 反序列化字段
        void DeserializeFields(IReadStream readStream);
        // 消息处理方法
        void Handle(ulong remoteMessageId, IPeer peer,IReadStream readStream);
    }
}