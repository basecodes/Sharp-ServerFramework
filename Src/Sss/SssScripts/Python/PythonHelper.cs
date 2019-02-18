using System;
using System.Collections.Generic;
using System.Reflection;
using IronPython.Compiler;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Hosting;

namespace Sss.SssScripts.Python {
    public class PythonHelper {
        private readonly ScriptEngine _scriptEngine;
        private ScriptScope _scriptScope;

        public PythonHelper() {
            var options = new Dictionary<string, object>();
            options["Debug"] = true;
            _scriptEngine =IronPython.Hosting.Python.CreateEngine(options);
        }

        public void SetVariable<T>(string name,T value) {
            _scriptScope.SetVariable(name, value);
        }

        public void SetSysVariable<T>(string name,T value) {
            var sys = _scriptEngine.GetSysModule();
            sys.SetVariable(name, value);
        }

        public void SetGlobalVariable<T>(string name,T value) {
            _scriptEngine.Runtime.Globals.SetVariable(name, value);
        }

        public dynamic GetBuiltinVariable(string name) {
            return _scriptEngine.GetBuiltinModule().GetVariable(name);
        }

        public void LoadAssembly(Assembly assembly) {
            _scriptEngine.Runtime.LoadAssembly(assembly);
        }

        public void SetSearchPaths(params string[] paths) {
            _scriptEngine.SetSearchPaths(paths);
        }

        public dynamic Create(dynamic type, params object[] args) {
            try {
                return _scriptEngine.Operations.CreateInstance(type, args);
            } catch (Exception e) {
                var eo = _scriptEngine.GetService<ExceptionOperations>();
                Console.Write(eo.FormatException(e));
            }

            return null;
        }

        public static string GetPythonTypeName(dynamic obj) {
            return DynamicHelpers.GetPythonType(obj).__name__;
        }

        public static string GetPythonTypeName(PythonType pythonType) {
            return (pythonType as dynamic).__name__;
        }

        public static PythonType GetPythonType(dynamic obj) {
            return DynamicHelpers.GetPythonType(obj);
        }
        

        public object Call(object obj, params object[] parameters) {
            return _scriptEngine.Operations.Invoke(obj, parameters);
        }

        public T Create<T>(PythonType type, params object[] args) {
            try {
                return (T) _scriptEngine.Operations.CreateInstance(type, args);
            } catch (Exception e) {
                var eo = _scriptEngine.GetService<ExceptionOperations>();
                Console.Write(eo.FormatException(e));
            }

            return default(T);
        }

        public dynamic Execute(string fileName, string entry) {
            var source = _scriptEngine.CreateScriptSourceFromFile(fileName);
            _scriptScope = _scriptEngine.CreateScope();

            try {
                source.Execute(_scriptScope);
            } catch (Exception e) {
                var eo = _scriptEngine.GetService<ExceptionOperations>();
                throw new Exception(eo.FormatException(e));
            }

            var pythonType = _scriptScope.GetVariable(entry);
            var startup = Create(pythonType);
            return startup.Main();
        }

        public IList<string> GetMembers(dynamic obj) {
            return _scriptEngine.Operations.GetMemberNames(obj);
        }

        public dynamic GetMember(dynamic obj,string name) {
            return _scriptEngine.Operations.GetMember(obj,name);
        }

        public void Execute(string fileName) {
            var source = _scriptEngine.CreateScriptSourceFromFile(fileName);
            _scriptScope = _scriptEngine.CreateScope();

            var compilerOptions = (PythonCompilerOptions) _scriptEngine.GetCompilerOptions(_scriptScope);
            compilerOptions.ModuleName = "__main__";
            compilerOptions.Module |= ModuleOptions.Initialize;
            var compiled = source.Compile(compilerOptions);
            
            try {
                compiled.Execute(_scriptScope);
            } catch (Exception e) {
                var eo = _scriptEngine.GetService<ExceptionOperations>();
                Console.Write(eo.FormatException(e));
            }
        }
    }
}