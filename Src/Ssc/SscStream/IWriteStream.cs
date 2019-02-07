using System;

namespace Ssc.SscStream {
    public interface IWriteStream : IStream {
        void ShiftLeft(ByteFragment value);
        void ShiftLeft<T>(T value) where T : IConvertible;
        void ShiftLeft(TypeCode typeCode, IConvertible value);
        void ShiftLeft(byte[] bytes);
        void ShiftLeft(byte[] bytes, int offset);
        void ShiftLeft(byte[] bytes, int offset, int count);


        void SetWriteOffset(int offset);


        void ShiftRight(ByteFragment value);
        void ShiftRight<T>(T value) where T : IConvertible;
        void ShiftRight(TypeCode typeCode, IConvertible value);
        void ShiftRight(byte[] bytes);
        void ShiftRight(byte[] bytes, int offset);
        void ShiftRight(byte[] bytes, int offset, int count);
    }
}