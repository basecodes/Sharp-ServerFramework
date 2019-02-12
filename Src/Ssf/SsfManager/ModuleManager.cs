using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using Ssc.SscAlgorithm;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssm.SsmModule;
using Sss.SssScripts.Lua;
using Sss.SssScripts.Pythons;

namespace Ssf.SsfManager {
    public class ModuleManager {
        private static readonly Logger Logger = LogManager.GetLogger<ModuleManager>(LogType.Middle);

        private readonly KeyList<string,IModule> _initializedModules;
        private readonly KeyList<string,IModule> _uninitializedModules;
        public ModuleManager() {
            _initializedModules = new KeyList<string,IModule>();
            _uninitializedModules = new KeyList<string,IModule>();
            LuaHelper.RegisterType<ModuleManager>();
        }

        public void AddModuleFromDllFile(string fileName, string entry = "Startup") {
            if (!File.Exists(fileName)) {
                throw new Exception($"{fileName}文件不存在！");
            }

            var asm = Assembly.LoadFrom(fileName);
            var type = asm.GetType(entry);
            if (type == null) {
                throw new Exception($"没有{entry}类！");
            }

            var obj = ObjectFactory.GetActivator<Entry>(type.GetConstructors().First())();
            if (obj == null) {
                throw new Exception($"dll库中未找到{entry}类");
            }

            IModule module = null;
            try {
                module = obj.Main();
            } catch (Exception e) {
                Logger.Error($"调用{entry}类的函数Main发生异常！" + e);
                return;
            }

            if (module == null) {
                throw new Exception($"{entry}.Main()返回空!");
            }

            AddModule(module);
        }

        public void AddModuleFromPythonFile(string fileName, string entry = "Startup", params string[] paths) {
            var pythonHelper = new PythonHelper();

            pythonHelper.SetSearchPaths(paths);
            pythonHelper.SetSysVariable("PythonHelper", pythonHelper);
            var module = pythonHelper.Execute(fileName, entry);

            if (module == null) {
                throw new Exception($"{fileName}库返回值异常，返回值为null!");
            }

            if (!typeof(IModule).IsAssignableFrom(PythonHelper.GetPythonType(module))) {
                throw new Exception($"模块类型非{nameof(IModule)}类型或者子类型！");
            }

            var type = PythonHelper.GetPythonTypeName(module);

            AddModule(type, module);
        }

        public void AddModuleFromLuaFile(string fileName, string entry, string libPath = "./?;./?.lua;./Lua/?;./Lua/?.lua") {
            var luaHelper = new LuaHelper();

            LuaGlobals.Init();

            luaHelper.SetSearchPath(libPath);
            luaHelper.RegisterGlobal(nameof(LuaHelper), luaHelper);

            var table = luaHelper.Execute(fileName)?.Table;
            if (table == null) {
                return;
            }

            var module = luaHelper.Call(table, nameof(Entry.Main)).Table;
            if (module == null) {
                throw new Exception($"{nameof(module)}为空！");
            }

            var dynValue = module.Get(nameof(IModule));
            if (dynValue == null || dynValue == DynValue.Nil) {
                throw new Exception($"未找到{nameof(IModule)}！");
            }

            var luaModule = dynValue.ToObject<IModule>();
            if (luaModule == null) {
                throw new Exception($"转换{nameof(IModule)}失败！");
            }

            AddModule(luaModule);
        }

        public void AddModule(string typeName,IModule module) {
            if (_uninitializedModules.ContainsKey(typeName) 
                || _initializedModules.ContainsKey(typeName)) {
                Logger.Warn($"{typeName}模块已存在!");
                return;
            }

            _uninitializedModules.AddItem(typeName, module);
        }

        public void AddModule(IModule module) {
            AddModule(module.GetType().Name, module);
        }

        public T AddModule<T>(params object[] args) where T : class,IModule {
            var type = typeof(T);
            var module = ObjectFactory.GetActivator<T>(typeof(T).GetConstructors().First())(args);
            AddModule(type.Name, module);
            return module;
        }

        public bool ContainsModule<T>() where T :class,IModule {
            var type = typeof(T);
            return _initializedModules.ContainsKey(type.Name) || _uninitializedModules.ContainsKey(type.Name);
        }

        public T GetModule<T>() where T : class,IModule {
            return GetModule(typeof(T).Name) as T;
        }

        public T GetModule<T>(string typeName) where T :  class,IModule {
            return GetModule(typeName) as T;
        }

        public T GetModule<T>(Type type) where T : class,IModule {
            return GetModule(type.Name) as T;
        }

        public IModule GetModule(Type type) {
            return GetModule(type.Name);
        }

        public IModule GetModule(string typeName) {
            return _initializedModules.GetValue(typeName);
        }

        public bool RemoveModule<T>() where T :class,IModule {
            return RemoveModule(typeof(T).Name);
        }

        public bool RemoveModule(string typeName) {
            var module = _initializedModules.RemoveItem(typeName);
            if (module != null) {
                module.Dispose();
                return true;
            }

            Logger.Warn($"移除{typeName}模块失败！");
            return false;
        }

        public IEnumerable<KeyValuePair<string,IModule>> ForeachUninitializedModule() {
            var e = _uninitializedModules.GetEnumerator();

            while (e.MoveNext()) {
                var module = e.Current;
                yield return module;
            }
        }

        public void ChangeUninitializedModuleState(string moduleName) {
            var serverModule = _uninitializedModules.RemoveItem(moduleName);
            _initializedModules.AddItem(moduleName, serverModule);
        }

        public IEnumerable<KeyValuePair<string,IModule>> ForeachInitializedModule() {
            foreach (var module in _initializedModules) {
                yield return module;
            }
        }
    }
}