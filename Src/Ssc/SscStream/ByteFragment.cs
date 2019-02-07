using System;

namespace Ssc.SscStream {
    public struct ByteFragment {
        public byte[] Buffer { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }

        public void SetByte(int offset, byte value) {
            if (Buffer == null) throw new ArgumentNullException(nameof(Buffer));
            if (offset < 0 || offset >= Buffer.Length) throw new IndexOutOfRangeException(nameof(offset));
            Buffer[offset] = value;
        }
    }
}