using System;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Ssc.SscRpc {
    public struct RpcResponse : IResponseMessage {
        public string Exception { get; private set; }
        public string MethodId { get; private set; }
        public ulong MessageId { get; private set; }

        private static readonly Logger Logger = LogManager.GetLogger<RpcResponse>(LogType.Middle);

        public RpcResponse(string exception, string methodId,ulong messageId) {
            Exception = exception;
            MethodId = methodId;
            MessageId = messageId;
        }

        public void DeserializeFields(IReadStream readStream) {
            Exception = readStream.ShiftRight<string>();
            MethodId = readStream.ShiftRight<string>();
            MessageId = readStream.ShiftRight<ulong>();
        }

        public void SerializeFields(IWriteStream writeStream) {
            writeStream.ShiftLeft(MessageId);
            writeStream.ShiftLeft(MethodId);
            writeStream.ShiftLeft(Exception);
        }

        public void Handle(ulong remoteMessageId,IPeer peer,IReadStream readStream) {
            if (peer == null) {
                Logger.Error($"{nameof(peer)} 为 null");
                return;
            }

            try {
                peer.Response(MessageId, this, new Deserializable(readStream));
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        }
    }
}