using Common.CSharp;
using Ssc.SscSerialization;
using Ssc.SscTemplate;

namespace CSharpModule.Packets {
    public class TestPacket : PoolAllocator<ITestPacket>, ITestPacket {
        public string Name { get; set; }
        public string Password { get; set; }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            Name = reader.Read<string>();
            Password = reader.Read<string>();
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            writer.Write(Name);
            writer.Write(Password);
        }

        public override string ToString() {
            return Name + " " + Password;
        }
    }
}
