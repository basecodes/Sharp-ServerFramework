using System;

namespace Ssc.SscExtension {
    public static class EnumExtension {
        public static T GetValue<T>(this Enum obj) 
            where T:struct,IConvertible {
            var value = Convert.ChangeType(obj, typeof(T));
            return (T)value;
        }
    }
}
