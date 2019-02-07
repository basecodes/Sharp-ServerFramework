using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssf.SsfNetwork.Packets {
    internal class Packet {
        private static readonly Logger Logger = LogManager.GetLogger<Packet>(LogType.Low);

        public static void ToBytes(PacketHead packetHead,IWriteStream writeSteam) {
            packetHead.ToBytes(writeSteam);
        }

        public static PacketHead Unpacking(IReadStream readStream) {
            var packetHead = new PacketHead();
            packetHead.FromBytes(readStream);
            return packetHead;
        }
    }
}