using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssc.SscStream;

namespace Ssf.SsfNetwork.Messages {
    public struct RawMessage :ISerializablePacket{

        private static readonly Logger Logger = LogManager.GetLogger<RawMessage>(LogType.Middle);

        public ushort OpCode { get; set; }
        public string Exception { get; set; }

        public IDeserializable Deserializable { get; private set; }
        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.Write(Exception);
            writer.Write(OpCode);
        }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            OpCode = reader.Read<ushort>();
            Exception = reader.Read<string>();
//             if (string.IsNullOrEmpty(Exception))
//                 SingleDeserializable = new SingleDeserializable(readStream);
//             else
//                 Logger.Error($"操作码{OpCode}远程调用出现错误！错误：" + Exception);
        }

        public void Handle(ulong messageId, IPeer peer,IReadStream readStream) {
            if (peer == null) {
                Logger.Error($"{nameof(peer)} 为 null");
                return;
            }

            
        }
    }
}