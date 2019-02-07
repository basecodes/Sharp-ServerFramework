using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;

namespace Sss.SssInteractives {
    public sealed class LuaIteractive : IIteractive {
        public string Language { get; } = "Lua";

        public void Run<T>(T startup, string exit, string[] args) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("用法：");
            Console.WriteLine($" >>  module = Startup.ModuleManager.GetModule(\"LoginModule\")");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            var script = new Script(CoreModules.Preset_Complete);
            var interpreter = new ReplInterpreter(script) {
                HandleDynamicExprs = true,
                HandleClassicExprsSyntax = true
            };
            
            script.Globals["Startup"] = startup;

            while (true) {
                Console.Write("$Ssf-lua " + interpreter.ClassicPrompt + " ");
                var s = Console.ReadLine();

                try {
                    var result = interpreter.Evaluate(s);

                    if (result != null && result.Type != DataType.Void) {
                        Console.WriteLine("{0}", result);
                    }
                } catch (InterpreterException ex) {
                    Console.WriteLine("{0}", ex.DecoratedMessage ?? ex.Message);
                } catch (Exception ex) {
                    Console.WriteLine("{0}", ex.Message);
                }
            }
        }
    }
}