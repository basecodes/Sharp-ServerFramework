using System;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Sss.SssInteractives {
    public sealed class PythonIteractive : IIteractive {
        public string Language => "Python";

        public void Run<T>(T startup, string exit, string[] args) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("用法：");
            Console.WriteLine($" >>  module = Startup.ModuleManager.GetModule(\"LoginModule\")");
            Console.WriteLine("-----------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;

            var code = "";
            Console.WriteLine();
            Console.Write("$Ssf-python >");
            var scriptEngine = Python.CreateEngine();
            var scriptScope = scriptEngine.CreateScope();

            scriptScope.SetVariable("Startup", startup);

            while (true) {
                var line = Console.ReadLine();
                code += line;
                var script = scriptEngine.CreateScriptSourceFromString(code, SourceCodeKind.InteractiveCode);
                var properties = script.GetCodeProperties();

                if (properties == ScriptCodeParseResult.IncompleteStatement 
                    || properties == ScriptCodeParseResult.IncompleteToken) {
                    if (!string.IsNullOrEmpty(line)) {
                        Console.Write("$Ssf-python >>");
                        continue;
                    }
                }
                
                try {
                    var compiledCode = script.Compile();
                    compiledCode.Execute(scriptScope);
                } catch (Exception e) {
                    var eo = scriptEngine.GetService <ExceptionOperations>();
                    Console.Write(eo.FormatException(e));
                }
                code = "";
                Console.Write("$Ssf-python >");
            }
        }
    }
}