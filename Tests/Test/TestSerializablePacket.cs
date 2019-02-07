using NUnit.Framework;
using Ssc.Ssc;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;
using System;

namespace Test {
    public interface IPlayer : ISerializablePacket,IRecyclable,IAssignable {
        string UserName { get; }
        int ID { get; }
        bool Sex { get; }
        string Signature { get; }
        int HeadId { get; }
        string MobilePhone { get; }
        int CardCount { get; }
        int PlayTimes { get; }
    }

    public class Player : PoolAllocator<IPlayer>, IPlayer {
        public string UserName { get; set; }
        public int ID { get; set; }
        public bool Sex { get; set; }
        public string Signature { get; set; }
        public int HeadId { get; set; }
        public string MobilePhone { get; set; }
        public int CardCount { get; set; }
        public int PlayTimes { get; set; }

        public void Dispose() {
        }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            UserName = reader.Read<string>();
            ID = reader.Read<int>();
            Sex = reader.Read<bool>();
            Signature = reader.Read<string>();
            HeadId = reader.Read<int>();
            MobilePhone = reader.Read<string>();
            CardCount = reader.Read<int>();
            PlayTimes = reader.Read<int>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.Write(UserName);
            writer.Write(ID);
            writer.Write(Sex);
            writer.Write(Signature);
            writer.Write(HeadId);
            writer.Write(MobilePhone);
            writer.Write(CardCount);
            writer.Write(PlayTimes);
        }
    }
    [TestFixture]
    public class TestSerializablePacket {
        [Test]
        public void Test() {
            var player = new Player();
            player.UserName = "Test";
            player.ID = 100;
            player.Sex = true;
            player.Signature = "签名";
            player.HeadId = 100;
            player.MobilePhone = "100086";
            player.CardCount = 5000;
            player.PlayTimes = 40000;

            PoolAllocator<IWriteStream>.SetPool((arguments => new WriteStream()));
            var writeStream = PoolAllocator<IWriteStream>.GetObject();
            var writer = new EndianBinaryWriter(writeStream);
            player.ToBinaryWriter(writer);

            var byteFragment = writeStream.ToByteFragment();

            PoolAllocator<IReadStream>.SetPool((arguments => new ReadStream()));
            var readStream = PoolAllocator<IReadStream>.GetObject();
            readStream.CopyBuffer(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count);
            var reader = new EndianBinaryReader(readStream);

            var check = new Player();
            check.FromBinaryReader(reader);

            Assert.That(check.UserName,Is.EqualTo(player.UserName));
            Assert.That(check.ID,Is.EqualTo(player.ID));
            Assert.That(check.Sex,Is.EqualTo(player.Sex));
            Assert.That(check.Signature,Is.EqualTo(player.Signature));
            Assert.That(check.HeadId,Is.EqualTo(player.HeadId));
            Assert.That(check.MobilePhone,Is.EqualTo(player.MobilePhone));
            Assert.That(check.CardCount,Is.EqualTo(player.CardCount));
            Assert.That(check.PlayTimes,Is.EqualTo(player.PlayTimes));
        }
    }
}
