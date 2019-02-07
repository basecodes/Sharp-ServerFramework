using System;
using Ssc.SscConfiguration;

namespace Ssc.SscExtension {
    public static class StringExtension {
        public static T ToValue<T>(this string value) where T : struct, IConvertible {
            return (T) value.ToValue(typeof(T).GetTypeCode());
        }

        public static int GetSize(this string value) {
            if (string.IsNullOrEmpty(value)) return sizeof(ushort);
            return OtherConfiguration.Encoding.GetByteCount(value) + sizeof(ushort);
        }


        public static byte[] ToBytes(this string value) {
            if (value == null || string.IsNullOrEmpty(value)) return new byte[2];
            var bytes = OtherConfiguration.Encoding.GetBytes(value);
            var buffer = new byte[bytes.Length + sizeof(ushort)];
            var lengthBytes = ((ushort) bytes.Length).ToBytes();
            Buffer.BlockCopy(lengthBytes, 0, buffer, 0, lengthBytes.Length);
            Buffer.BlockCopy(bytes, 0, buffer, lengthBytes.Length, bytes.Length);
            return buffer;
        }

        public static byte[] ToBytes2(this string value) {
            if (value == null || string.IsNullOrEmpty(value)) return null;
            var bytes = OtherConfiguration.Encoding.GetBytes(value);
            var buffer = new byte[bytes.Length];
            var lengthBytes = ((ushort)bytes.Length).ToBytes();
            Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
            return buffer;
        }

        private static IConvertible ToValue(this string value, TypeCode typeCode) {
            if (typeCode == TypeCode.Boolean) return Convert.ToBoolean(value);

            if (typeCode == TypeCode.Char) return Convert.ToChar(value);

            if (typeCode == TypeCode.SByte) return Convert.ToSByte(value);

            if (typeCode == TypeCode.Byte) return Convert.ToByte(value);

            if (typeCode == TypeCode.Int16) return Convert.ToInt16(value);

            if (typeCode == TypeCode.UInt16) return Convert.ToUInt16(value);

            if (typeCode == TypeCode.Int32) return Convert.ToInt32(value);

            if (typeCode == TypeCode.UInt32) return Convert.ToUInt32(value);

            if (typeCode == TypeCode.Int64) return Convert.ToInt64(value);

            if (typeCode == TypeCode.UInt64) return Convert.ToUInt64(value);

            if (typeCode == TypeCode.Single) return Convert.ToSingle(value);

            if (typeCode == TypeCode.Double) return Convert.ToDouble(value);

            throw new NotSupportedException("TypeCode:" + typeCode);
        }
    }
}