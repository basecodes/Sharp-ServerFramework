using System;

namespace Ssc.SscExtension {
    public static class TypeExtension {
        public static TypeCode GetTypeCode(this Type type) {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return Type.GetTypeCode(type);
        }
    }
}