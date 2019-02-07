using Ssc.SscStream;

namespace Ssu.SsuNetwork.DataPacks {
    internal class Packet {
        //private readonly static Logger Logger = LogManager.GetLogger<DataPack>(LogType.Low);

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