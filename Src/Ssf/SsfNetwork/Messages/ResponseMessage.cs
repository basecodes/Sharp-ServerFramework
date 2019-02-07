using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssf.SsfNetwork.Messages {
    public class ResponseMessage : IResponseMessage {
        private static readonly Logger Logger = LogManager.GetLogger<ResponseMessage>(LogType.Middle);

        public bool Status { get; set; }
        public virtual short ID { get; protected set; }
        public string Exception { get; internal set; }

        public void SerializeFields(IWriteStream writeStream) {
            writeStream.ShiftLeft(Status);
            writeStream.ShiftLeft(Exception);
            writeStream.ShiftLeft(ID);
        }

        public void DeserializeFields(IReadStream readStream) {
            ID = readStream.ShiftRight<short>();
            Exception = readStream.ShiftRight<string>();
            Status = readStream.ShiftRight<bool>();
        }

        public void Handle(ulong remoteMessageId,IPeer player,IReadStream readStream) {
            if (player == null) {
                Logger.Error($"{nameof(player)} == null");
            }
        }
    }
}