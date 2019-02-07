using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SourceCodeKind = Microsoft.CodeAnalysis.SourceCodeKind;

namespace Sss.SssInteractives {
    public sealed class CSharpIteractive : IIteractive {
        public string Language => "C#";

        public CSharpIteractive() {
        }

        public void Run<T>(T startup, string exit, string[] args) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("用法：");
            Console.WriteLine($" >>  var module = ModuleManager.GetModule(\"LoginModule\");");
            Console.WriteLine("-----------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;

            var code = "";
            Console.WriteLine();
            Console.Write("$Ssf-csharp>");

            ScriptState<object> scriptState = null;
            while (true) {
                var line = Console.ReadLine();
                code += line;
                var tree = SyntaxFactory.ParseSyntaxTree(code,
                    new CSharpParseOptions(kind: SourceCodeKind.Script,
                        languageVersion: LanguageVersion.Latest), "");

                if (!SyntaxFactory.IsCompleteSubmission(tree)) {
                    if (!string.IsNullOrEmpty(line)) {
                        Console.Write("$Ssf-csharp >>");
                        continue;
                    }
                }

                scriptState = scriptState == null ?
                    CSharpScript.RunAsync(
                        code, ScriptOptions.Default.AddImports("System"), startup, typeof(T)).Result :
                    scriptState.ContinueWithAsync(code).Result;
                code = "";
                Console.Write("$Ssf-csharp >");
            }
        }
    }
}