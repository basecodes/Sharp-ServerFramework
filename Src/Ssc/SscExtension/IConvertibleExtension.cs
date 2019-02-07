using System;

namespace Ssc.SscExtension {
    public static class IConvertibleExtension {
        public static int GetSize(this IConvertible value, TypeCode typeCode) {
            if (typeCode == TypeCode.String) return ((string) value).GetSize();

            var length = typeCode.GetSize();
            return length;
        }

        public static int CopyTo(this IConvertible value, TypeCode typeCode, byte[] buffer, int offset) {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var size = value.GetSize(typeCode);
            if (buffer.Length - offset < size) throw new ArgumentException(nameof(buffer) + "缓冲区太小！");

            var bytes = value.ToBytes(typeCode);
            Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
            return bytes.Length;
        }

        public static byte[] ToBytes(this IConvertible value, TypeCode typeCode) {
            if (typeCode == TypeCode.Boolean) return BitConverter.GetBytes(value.ToBoolean(null));

            if (typeCode == TypeCode.Char) {
                var b = Convert.ToByte(value.ToChar(null));
                return new[] {b};
            }

            if (typeCode == TypeCode.SByte) {
                var b = Convert.ToByte(value.ToSByte(null));
                return new[] {b};
            }

            if (typeCode == TypeCode.Byte) {
                var b = Convert.ToByte(value.ToByte(null));
                return new[] {b};
            }

            if (typeCode == TypeCode.Int16) return BitConverter.GetBytes(value.ToInt16(null));

            if (typeCode == TypeCode.UInt16) return BitConverter.GetBytes(value.ToUInt16(null));

            if (typeCode == TypeCode.Int32) return BitConverter.GetBytes(value.ToInt32(null));

            if (typeCode == TypeCode.UInt32) return BitConverter.GetBytes(value.ToUInt32(null));

            if (typeCode == TypeCode.Int64) return BitConverter.GetBytes(value.ToInt64(null));

            if (typeCode == TypeCode.UInt64) return BitConverter.GetBytes(value.ToUInt64(null));

            if (typeCode == TypeCode.Single) return BitConverter.GetBytes(value.ToSingle(null));

            if (typeCode == TypeCode.Double) return BitConverter.GetBytes(value.ToDouble(null));

            if (typeCode == TypeCode.String) return ((string) value).ToBytes();

            throw new NotSupportedException("TypeCode:" + typeCode);
        }
        
        public static T ToEnum<T>(this IConvertible convertible) 
            where T:Enum{
            var value = Enum.ToObject(typeof(T) , convertible);
            return (T)value;
        }
    }
}