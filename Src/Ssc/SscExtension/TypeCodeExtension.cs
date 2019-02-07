using System;

namespace Ssc.SscExtension {
    public static class TypeCodeExtension {
        public static Type GetBaseType(this TypeCode typeCode) {
            if (typeCode == TypeCode.Boolean) return typeof(bool);

            if (typeCode == TypeCode.Char) return typeof(char);

            if (typeCode == TypeCode.SByte) return typeof(sbyte);

            if (typeCode == TypeCode.Byte) return typeof(byte);

            if (typeCode == TypeCode.Int16) return typeof(short);

            if (typeCode == TypeCode.UInt16) return typeof(ushort);

            if (typeCode == TypeCode.Int32) return typeof(int);

            if (typeCode == TypeCode.UInt32) return typeof(uint);

            if (typeCode == TypeCode.Int64) return typeof(long);

            if (typeCode == TypeCode.UInt64) return typeof(ulong);

            if (typeCode == TypeCode.Single) return typeof(float);

            if (typeCode == TypeCode.Double) return typeof(double);

            if (typeCode == TypeCode.String) return typeof(string);

            throw new NotSupportedException(typeCode.ToString());
        }

        public static int GetSize(this TypeCode typeCode) {
            if (typeCode == TypeCode.Boolean) return sizeof(bool);

            if (typeCode == TypeCode.Char) return sizeof(char);

            if (typeCode == TypeCode.SByte) return sizeof(sbyte);

            if (typeCode == TypeCode.Byte) return sizeof(byte);

            if (typeCode == TypeCode.Int16) return sizeof(short);

            if (typeCode == TypeCode.UInt16) return sizeof(ushort);

            if (typeCode == TypeCode.Int32) return sizeof(int);

            if (typeCode == TypeCode.UInt32) return sizeof(uint);

            if (typeCode == TypeCode.Int64) return sizeof(long);

            if (typeCode == TypeCode.UInt64) return sizeof(ulong);

            if (typeCode == TypeCode.Single) return sizeof(float);

            if (typeCode == TypeCode.Double) return sizeof(double);

            if (typeCode == TypeCode.String) return sizeof(ushort);

            throw new NotSupportedException(typeCode.ToString());
        }
    }
}