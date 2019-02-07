using System;

namespace Ssc.SscExtension {
    public static class StructExtension {
        public static int GetStructSize<T>(this T value) where T : struct, IConvertible {
            var length = value.GetTypeCode().GetSize();
            return length;
        }

        public static byte[] ToBytes<T>(this T obj) where T : struct, IConvertible {
            return obj.ToBytes(typeof(T).GetTypeCode());
        }

        public static byte[] ToBytes<T>(this T obj, TypeCode typeCode) where T : struct, IConvertible {
            if (typeCode == TypeCode.Boolean) return BitConverter.GetBytes((bool) (IConvertible) obj);

            if (typeCode == TypeCode.Char) return new[] {Convert.ToByte((char) (IConvertible) obj)};

            if (typeCode == TypeCode.SByte) return new[] {Convert.ToByte((sbyte) (IConvertible) obj)};

            if (typeCode == TypeCode.Byte) return new[] {Convert.ToByte((byte) (IConvertible) obj)};

            if (typeCode == TypeCode.Int16) return BitConverter.GetBytes((short) (IConvertible) obj);

            if (typeCode == TypeCode.UInt16) return BitConverter.GetBytes((ushort) (IConvertible) obj);

            if (typeCode == TypeCode.Int32) return BitConverter.GetBytes((int) (IConvertible) obj);

            if (typeCode == TypeCode.UInt32) return BitConverter.GetBytes((uint) (IConvertible) obj);

            if (typeCode == TypeCode.Int64) return BitConverter.GetBytes((long) (IConvertible) obj);

            if (typeCode == TypeCode.UInt64) return BitConverter.GetBytes((ulong) (IConvertible) obj);

            if (typeCode == TypeCode.Single) return BitConverter.GetBytes((float) (IConvertible) obj);

            if (typeCode == TypeCode.Double) return BitConverter.GetBytes((double) (IConvertible) obj);

            throw new NotSupportedException("TypeCode:" + typeCode);
        }
    }
}