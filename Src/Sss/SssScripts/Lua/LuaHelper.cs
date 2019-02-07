using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using Ssc.SscLog;

namespace Sss.SssScripts.Lua {
    public class LuaHelper {
        private static readonly Logger Logger = LogManager.GetLogger<LuaHelper>(LogType.Middle);

        private readonly Script _script;

        public LuaHelper() {
            _script = new Script();

            LuaGlobals.Init();
            
            UserStatic<TypeCode>();
            UserStatic<LuaProxy>();
            UserStatic<LuaLoader>();
        }

        public static void RegisterType<T>() {
            UserData.RegisterType<T>();
        }

        public static void RegisterType(Type type) {
            UserData.RegisterType(type);
        }

        public void RegisterGlobal(string name, object value) {
            _script.Globals[name] = value;
        }

        public void UserStatic<T>() {
            RegisterType<T>();
            _script.Globals[typeof(T).Name] = UserData.CreateStatic<T>();
        }

        public void UserStatic(Type type) {
            RegisterType(type);
            _script.Globals[type.Name] = UserData.CreateStatic(type);
        }

        public void SetSearchPath(string paths = "./?;./?.lua") {
            ((ScriptLoaderBase) _script.Options.ScriptLoader).ModulePaths =
                ScriptLoaderBase.UnpackStringPaths(paths);
        }

        public DynValue Execute(string fileName) {
            try {
                 return _script.DoFile(fileName);
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            return null;
        }

        public DynValue Call(Table template, string methodName, params object[] args) {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentException(nameof(methodName));
            
            var func = Get(template, methodName);
            if (func == DynValue.Nil) throw new NotImplementedException(methodName);

            return _script.Call(func, args);
        }

        public static DynValue Get(Table template, string methodName) {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentException(nameof(methodName));
            
            var func = template.Get(methodName);
            if (func == DynValue.Nil) {
                if (template.MetaTable == null) return DynValue.Nil;
                var table = template.MetaTable.Get("__index");
                if (table == DynValue.Nil) return DynValue.Nil;
                return Get(table.Table, methodName);
            }

            return func;
        }

        public DynValue Call(Table template, string methodName) {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentException(nameof(methodName));
            
            var func = Get(template, methodName);
            if (func == DynValue.Nil) throw new NotImplementedException(methodName);

            return _script.Call(func);
        }
        
        public DynValue Call(DynValue function, params object[] args) {
            if (function == null) throw new ArgumentNullException(nameof(function));

            try {
                return _script.Call(function,args);
            } catch (ScriptRuntimeException e) {
                Console.WriteLine(e.DecoratedMessage);
            }
            return DynValue.Nil;
        }
    }
}