using Ssc.SscStream;
using Ssf.SsfNetwork.Sockets;

namespace Ssf.SsfNetwork.Packets {
    internal struct PacketHead {
        // 包总长度
        public ushort Length { get; set; }
        // 消息ID
        public ulong PacketId { get; set; }
        // 包从属消息
        public SendOption SendOption { get; set; }
        // 消息长度
        public ushort TotalBytes { get; set; }
        // 片段ID
        public ushort FragmentId { get; set; }
        // 总片段数
        public ushort TotalFragments { get; set; }

        public static int GetSize() => sizeof(ushort) + sizeof(ulong)+ sizeof(SendOption) + sizeof(ushort)
                                + sizeof(ushort) + sizeof(ushort);
        public void ToBytes(IWriteStream writeSteam) {
            writeSteam.ShiftLeft(TotalFragments);
            writeSteam.ShiftLeft(FragmentId);
            writeSteam.ShiftLeft(TotalBytes);
            writeSteam.ShiftLeft((byte) SendOption);
            writeSteam.ShiftLeft(PacketId);
            writeSteam.ShiftLeft(Length);
        }

        public void FromBytes(IReadStream readStream) {
            Length = readStream.ShiftRight<ushort>();
            PacketId = readStream.ShiftRight<ulong>();
            SendOption = (SendOption) readStream.ShiftRight<byte>();
            TotalBytes = readStream.ShiftRight<ushort>();
            FragmentId = readStream.ShiftRight<ushort>();
            TotalFragments = readStream.ShiftRight<ushort>();
        }
    }
}