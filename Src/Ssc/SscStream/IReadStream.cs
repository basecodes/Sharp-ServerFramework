using System;
using Ssc.Ssc;

namespace Ssc.SscStream {
    public interface IReadStream :ICloneable<IReadStream>,IStream {
        void SetReadOffset(int offset);
        void SetReadCount(int count);
        void CopyBuffer(byte[] buffer, int offset, int count);
        ByteFragment GetReadBuffer();
        IConvertible RightPeek(TypeCode typeCode);
        ByteFragment RightPeek(int count);
        T RightPeek<T>() where T : IConvertible;

        /// <summary>
        ///     string一律返回""
        /// </summary>
        IConvertible ShiftRight(TypeCode typeCode);

        T ShiftRight<T>() where T : IConvertible;

        ByteFragment ShiftRight(int count);
    }
}