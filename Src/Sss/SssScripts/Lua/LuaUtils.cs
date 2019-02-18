using System;
using System.Reflection;

namespace Sss.SssScripts.Lua {
    public class LuaUtils {

        static LuaUtils(){
            LuaHelper.RegisterType<Assembly>();
        }

        public static Assembly LoadAssembly(string fileName) {
            return Assembly.LoadFrom(fileName);
        }

        public static Type GetType(Assembly assembly,string name) {
            return assembly.GetType(name);
        }

        public static Type GetType(TypeCode typeCode) {
            return typeCode.GetType();
        }
    }
}
