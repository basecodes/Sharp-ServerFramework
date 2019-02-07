using Ssc.SscStream;

namespace Ssc.Ssc {
    public interface IMessageDispatcher {
        bool SendMessage(ulong id, IWriteStream writeStream);
        bool SendMessage(IWriteStream writeStream, ResponseCallback responseCallback);
    }
}