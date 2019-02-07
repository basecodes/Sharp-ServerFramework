using System;
using Ssc.SscExtension;
using Ssc.SscFactory;
using Ssc.SscTemplate;

namespace Ssc.SscStream {

    public class WriteStream :PoolAllocator<IWriteStream>, IWriteStream {
        //private static readonly Logger Logger = LogManager.GetLogger<WriteStream>(LogType.Middle);

        private byte[] _buffer;
        private int _count;
        private int _leftOffsetWrite;
        private int _rightOffsetWrite;
        private int _startOffset;

        public WriteStream() {
        }

        public override void Assign() {
            base.Assign();

            _buffer = ObjectFactory.CreateBuffer();
            _count = Ssci.StreamConfig.BufferSize - Ssci.StreamConfig.PacketOffset;

            _startOffset = Ssci.StreamConfig.PacketOffset;
            _leftOffsetWrite = Ssci.StreamConfig.PacketOffset;
            _rightOffsetWrite = Ssci.StreamConfig.PacketOffset;
        }
        public void Reset() {
            _leftOffsetWrite = _startOffset;
            _rightOffsetWrite = _startOffset;
        }

        public void Dispose() {
            Recycle();
        }

        public ByteFragment ToByteFragment() {
            var byteSegment = new ByteFragment {
                Buffer = _buffer, Offset = _leftOffsetWrite, Count = _rightOffsetWrite - _leftOffsetWrite
            };
            return byteSegment;
        }

        public void ShiftLeft(ByteFragment value) {
            ShiftLeft(value.Buffer, value.Offset, value.Count);
        }

        public void ShiftLeft<T>(T value) where T : IConvertible {
            ShiftLeft(typeof(T).GetTypeCode(), value);
        }

        public void ShiftLeft(TypeCode typeCode, IConvertible value) {
            if (_leftOffsetWrite < 0) throw new IndexOutOfRangeException("左偏移溢出！");

            var size = value.GetSize(typeCode);
            if (_leftOffsetWrite < size) throw new IndexOutOfRangeException("左偏移溢出！");

            if (WriteRemaining() < size) throw new IndexOutOfRangeException("缓冲区溢出！");

            _leftOffsetWrite -= size;
            value.CopyTo(typeCode, _buffer, _leftOffsetWrite);
        }

        public void ShiftLeft(byte[] bytes) {
            if (bytes == null || bytes.Length == 0) return;
            ShiftLeft(bytes, 0);
        }

        public void ShiftLeft(byte[] bytes, int offset) {
            if (bytes == null || bytes.Length == 0) return;
            if (offset < 0 || offset >= bytes.Length) throw new IndexOutOfRangeException(nameof(offset));
            ShiftLeft(bytes, offset, bytes.Length - offset);
        }

        public void ShiftLeft(byte[] bytes, int offset, int count) {
            if (bytes == null || bytes.Length == 0) return;

            if (count < 0 || count > bytes.Length - offset) throw new ArgumentOutOfRangeException(nameof(count));

            if (offset < 0 || offset >= bytes.Length) throw new IndexOutOfRangeException(nameof(offset));

            if (_leftOffsetWrite < count) throw new IndexOutOfRangeException("左偏移溢出！");

            if (WriteRemaining() < count) throw new IndexOutOfRangeException("缓冲区溢出！");

            _leftOffsetWrite -= count;
            Buffer.BlockCopy(bytes, offset, _buffer, _leftOffsetWrite, count);
        }

        private int WriteRemaining() {
            var count = Math.Abs(_leftOffsetWrite - _startOffset) + Math.Abs(_rightOffsetWrite - _startOffset);
            return _count - count;
        }

        public void SetWriteOffset(int offset) {
            if (offset > 0)
                _rightOffsetWrite += offset;
            else
                _rightOffsetWrite -= offset;
        }

        public void ShiftRight(ByteFragment value) {
            ShiftRight(value.Buffer, value.Offset, value.Count);
        }

        public void ShiftRight<T>(T value) where T : IConvertible {
            ShiftRight(typeof(T).GetTypeCode(), value);
        }

        public void ShiftRight(TypeCode typeCode, IConvertible value) {
            if (_rightOffsetWrite > _buffer.Length) throw new IndexOutOfRangeException("右偏移溢出！");

            var size = value.GetSize(typeCode);
            if (_rightOffsetWrite + size > _buffer.Length) throw new IndexOutOfRangeException("右偏移溢出！");

            if (WriteRemaining() < size) throw new IndexOutOfRangeException("缓冲区溢出！");

            _rightOffsetWrite += value.CopyTo(typeCode, _buffer, _rightOffsetWrite);
        }

        public void ShiftRight(byte[] bytes) {
            if (bytes == null || bytes.Length == 0) return;
            ShiftRight(bytes, 0);
        }

        public void ShiftRight(byte[] bytes, int offset) {
            if (bytes == null || bytes.Length == 0) return;

            if (offset < 0 || offset >= bytes.Length) throw new IndexOutOfRangeException(nameof(offset));

            ShiftRight(bytes, offset, bytes.Length - offset);
        }

        public void ShiftRight(byte[] bytes, int offset, int count) {
            if (bytes == null || bytes.Length == 0) return;

            if (count < 0 || count > bytes.Length - offset) throw new ArgumentOutOfRangeException(nameof(count));

            if (offset < 0 || offset >= bytes.Length) throw new IndexOutOfRangeException(nameof(offset));

            if (_rightOffsetWrite + count > _buffer.Length) throw new IndexOutOfRangeException("右偏移溢出！");

            if (WriteRemaining() < count) throw new IndexOutOfRangeException("缓冲区溢出！");

            Buffer.BlockCopy(bytes, offset, _buffer, _rightOffsetWrite, count);
            _rightOffsetWrite += count;
        }

        public override void Recycle() {
            base.Recycle();
            ObjectFactory.Recycle(_buffer);
        }
    }
}