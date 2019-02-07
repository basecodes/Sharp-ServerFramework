using System;
using Ssc.SscExtension;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscTemplate;

namespace Ssc.SscStream {
    /// <summary>
    /// 只能向右读取
    /// </summary>
    public class ReadStream : PoolAllocator<IReadStream>, IReadStream {

        private static readonly Logger Logger = LogManager.GetLogger<ReadStream>(LogType.Middle);

        private byte[] _buffer;
        private int _maxCount;
        private int _currentReadCount;

        private int _rightOffsetRead;
        private int _startOffset;

        public void Dispose() {
            Recycle();
        }

        public override void Recycle() {
            base.Recycle();

            ObjectFactory.Recycle(_buffer);
            _buffer = null;
            _maxCount = 0;
            _currentReadCount = 0;
            _rightOffsetRead = 0;
            _startOffset = 0;
        }

        public ReadStream() {
        }

        // 设置将要读取的缓冲区，在当前读的位置后设置
        public void CopyBuffer(byte[] buffer, int offset, int count) {
            if (buffer == null) {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0) {
                throw new IndexOutOfRangeException($"{nameof(offset)}索引参数不能为负！");
            }

            if (count < 0) {
                throw new ArgumentOutOfRangeException($"{nameof(offset)}长度参数不能为负！");
            }

            if (buffer.Length - offset < count) {
                 throw new ArgumentOutOfRangeException($"{nameof(buffer)}缓冲区数据低于期望大小！");
            }

            if (count > _buffer.Length - _rightOffsetRead) {
                throw new ArgumentOutOfRangeException($"数据长度大于缓存区容量!");
            }

            Buffer.BlockCopy(buffer, offset, _buffer, _rightOffsetRead, count);
            _currentReadCount = (_rightOffsetRead  -  _startOffset) + count;
        }

        // 分配对象调用
        public override void Assign() {
            base.Assign();

            _buffer = ObjectFactory.CreateBuffer();
            _maxCount = Ssci.StreamConfig.BufferSize - Ssci.StreamConfig.PacketOffset;
            _currentReadCount = _maxCount;

            _startOffset = Ssci.StreamConfig.PacketOffset;
            _rightOffsetRead = Ssci.StreamConfig.PacketOffset;
        }

        public void Reset() {
            _rightOffsetRead = _startOffset;
        }

        public ByteFragment ToByteFragment() {
            var byteFragment = new ByteFragment {
                Buffer = _buffer,
                Offset = _rightOffsetRead,
                Count = _currentReadCount - (_rightOffsetRead - _startOffset)
            };
            return byteFragment;
        }

        public ByteFragment GetReadBuffer() {
            var byteFragment = new ByteFragment {
                Buffer = _buffer,
                Offset = _startOffset,
                Count = _currentReadCount
            };
            return byteFragment;
        }

        private int ReadRemaining() {
            var count = Math.Abs(_rightOffsetRead - _startOffset);
            return _currentReadCount - count;
        }

        public IConvertible RightPeek(TypeCode typeCode) {
            var size = typeCode.GetSize();
            var remaining = ReadRemaining();
            if (remaining < size) {
                throw new IndexOutOfRangeException($"缓冲区溢出！剩余可用{remaining},期望{size}");
            }

            var value = _buffer.ToValue(_rightOffsetRead, typeCode);
            return value;
        }

        public ByteFragment RightPeek(int count) {
            var remaining = ReadRemaining();
            if (remaining < count) {
                throw new IndexOutOfRangeException($"缓冲区溢出！剩余可用{remaining},期望{count}");
            }

            var byteFragment = new ByteFragment {
                Buffer = _buffer,
                Offset = _rightOffsetRead,
                Count = count
            };
            return byteFragment;
        }

        public T RightPeek<T>() where T : IConvertible {
            var size = typeof(T).GetTypeCode().GetSize();
            var remaining = ReadRemaining();
            if (remaining < size) throw new IndexOutOfRangeException($"缓冲区溢出！剩余可用{remaining},期望{size}");
            var value = _buffer.ToValue<T>(_rightOffsetRead);
            return value;
        }

        public void SetReadOffset(int offset) {
            if (offset > 0)
                _rightOffsetRead += offset;
            else
                _rightOffsetRead -= offset;
        }

        public void SetReadCount(int count) {
            if (count > _maxCount) {
                throw new ArgumentOutOfRangeException($"{nameof(count)}长度太长！");
            }
            _currentReadCount = count;
        }

        public T ShiftRight<T>() where T : IConvertible {
            var value = RightPeek<T>();
            _rightOffsetRead += value.GetSize(typeof(T).GetTypeCode());
            return value;
        }

        public IConvertible ShiftRight(TypeCode typeCode) {
            var value = RightPeek(typeCode);
            _rightOffsetRead += value.GetSize(typeCode);
            return value;
        }

        public ByteFragment ShiftRight(int count) {
            var byteFragment = RightPeek(count);
            _rightOffsetRead += count;
            return byteFragment;
        }

        public IReadStream Clone() {
            var readStream = PoolAllocator<IReadStream>.GetObject() as ReadStream;
            Array.Copy(_buffer,readStream._buffer,_buffer.Length);
            readStream._maxCount = _maxCount;
            readStream._currentReadCount = _currentReadCount;
            readStream._rightOffsetRead = _rightOffsetRead;
            readStream._startOffset = _startOffset;
            return readStream;
        }
    }
}