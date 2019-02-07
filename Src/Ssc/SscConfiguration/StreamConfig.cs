using System;

namespace Ssc.SscConfiguration {
    [Serializable]
    public class StreamConfig {
        public int BufferSize = 1 << 14;
        public int BufferLength = 4096;
        public int PacketOffset = 128;

        public override string ToString() {
            var str = "";
            var fields = GetType().GetFields();
            foreach (var item in fields) {
                var value = item.GetValue(this);
                str += item.Name + " : " + value + "\n";
            }
            return str;
        }
    }
}