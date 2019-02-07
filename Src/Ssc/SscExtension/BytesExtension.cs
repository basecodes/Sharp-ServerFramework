using System;
using Ssc.SscConfiguration;

namespace Ssc.SscExtension {
    public static class BytesExtension {
        public static T ToValue<T>(this byte[] buffer) where T : IConvertible {
            if (buffer == null || buffer.Length == 0) return default;
            return buffer.ToValue<T>(0);
        }

        public static T ToValue<T>(this byte[] buffer, int startIndex) where T : IConvertible {
            if (buffer == null || buffer.Length == 0) return default;

            if (startIndex < 0 || buffer.Length <= startIndex) throw new IndexOutOfRangeException(nameof(startIndex));

            return (T) buffer.ToValue(startIndex, typeof(T).GetTypeCode());
        }

        public static string toString(this byte[] buffer, int offset) {
            if (buffer == null || buffer.Length == 0) return null;

            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            if (buffer.Length - offset < sizeof(ushort)) throw new ArgumentOutOfRangeException(nameof(offset));

            var length = buffer.ToValue<ushort>(offset);
            if (buffer.Length - sizeof(ushort) - length < 0) throw new ArgumentOutOfRangeException("缓冲区溢出！");

            offset += sizeof(ushort);
            var value = OtherConfiguration.Encoding.GetString(buffer, offset, length);
            return value;
        }

        public static string toString(this byte[] buffer) {
            return toString(buffer, 0);
        }

        #region ToConvertible

        internal static IConvertible ToValue(this byte[] buffer, TypeCode typeCode) {
            return buffer.ToValue(0, typeCode);
        }

        internal static IConvertible ToValue(this byte[] buffer, int startIndex, TypeCode typeCode) {
            if (startIndex < 0 || buffer.Length <= startIndex) throw new IndexOutOfRangeException(nameof(startIndex));

            if (typeCode == TypeCode.Boolean) {
                if (buffer.Length - startIndex < sizeof(bool)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToBoolean(buffer, startIndex);
            }

            if (typeCode == TypeCode.Char) {
                if (buffer.Length - startIndex < sizeof(char)) throw new IndexOutOfRangeException(nameof(startIndex));
                return buffer[startIndex];
            }

            if (typeCode == TypeCode.SByte) {
                if (buffer.Length - startIndex < sizeof(sbyte)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToChar(buffer, startIndex);
            }

            if (typeCode == TypeCode.Byte) {
                if (buffer.Length - startIndex < sizeof(byte)) throw new IndexOutOfRangeException(nameof(startIndex));
                return buffer[startIndex];
            }

            if (typeCode == TypeCode.Int16) {
                if (buffer.Length - startIndex < sizeof(short)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToInt16(buffer, startIndex);
            }

            if (typeCode == TypeCode.UInt16) {
                if (buffer.Length - startIndex < sizeof(ushort)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToUInt16(buffer, startIndex);
            }

            if (typeCode == TypeCode.Int32) {
                if (buffer.Length - startIndex < sizeof(int)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToInt32(buffer, startIndex);
            }

            if (typeCode == TypeCode.UInt32) {
                if (buffer.Length - startIndex < sizeof(uint)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToUInt32(buffer, startIndex);
            }

            if (typeCode == TypeCode.Int64) {
                if (buffer.Length - startIndex < sizeof(long)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToInt64(buffer, startIndex);
            }

            if (typeCode == TypeCode.UInt64) {
                if (buffer.Length - startIndex < sizeof(ulong)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToUInt64(buffer, startIndex);
            }

            if (typeCode == TypeCode.Single) {
                if (buffer.Length - startIndex < sizeof(float)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToSingle(buffer, startIndex);
            }

            if (typeCode == TypeCode.Double) {
                if (buffer.Length - startIndex < sizeof(double)) throw new IndexOutOfRangeException(nameof(startIndex));
                return BitConverter.ToDouble(buffer, startIndex);
            }

            if (typeCode == TypeCode.String) {
                if (buffer.Length - startIndex < sizeof(ushort)) throw new IndexOutOfRangeException(nameof(startIndex));
                return toString(buffer, startIndex);
            }

            throw new NotSupportedException("TypeCode:" + typeCode);
        }

        #endregion ToConvertible
    }
}