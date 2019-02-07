using System;
using System.Reflection;
using System.Runtime.Versioning;
using Sss.SssInteractives;

namespace Sss.SssManager {
    public class InteractiveManager {
        private static IIteractive _iteractive;
        private static Action _prompt;
        public static void Show(Action prompt) {
            _prompt = prompt;
        }
        public static void RunInteractive<T>(T startup, string[] args) {
            var framework = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            Console.WriteLine($"SSF REPL {3.0} [{framework}]");
            Console.WriteLine();
            Console.WriteLine("命令: ssf");
            Console.WriteLine();
            Console.WriteLine("     -l      [C#|Python|Lua]         选择交互语言");
            Console.WriteLine("     -exit                           退出");

            Console.WriteLine();
            _prompt?.Invoke();
            Console.WriteLine();

            while (true) {
                Console.Write("ssf > ");
                var input = Console.ReadLine();
                var cmd = input.Split(' ');
                if (cmd.Length == 0) continue;
                switch (cmd[0]) {
                    case "ssf":
                        if (cmd.Length == 1) continue;
                        switch (cmd[1]) {
                            case "-l":
                                if (cmd.Length == 2) continue;
                                switch (cmd[2]) {
                                    case "C#":
                                        _iteractive = new CSharpIteractive();
                                        break;

                                    case "Lua":
                                        _iteractive = new LuaIteractive();
                                        break;

                                    case "Python":
                                        _iteractive = new PythonIteractive();
                                        break;
                                }

                                break;
                            case "-exit":
                                return;
                        }

                        break;
                }


                _iteractive?.Run(startup, "-exit", args);
            }
        }
    }
}