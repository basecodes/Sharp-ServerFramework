using System;
using MoonSharp.Interpreter;

namespace Sss.SssScripts.Lua {
    internal static class LuaRegister {
        public static void RegisterFunc<T>() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Function, typeof(Func<T>),
                v => {
                    var function = v.Function;
                    return (Func<T>) (() => function.Call().ToObject<T>());
                }
            );
        }

        public static void RegisterFunc<T1, TResult>() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Function, typeof(Func<T1, TResult>),
                v => {
                    var function = v.Function;
                    return (Func<T1, TResult>) (p1 => function.Call(p1).ToObject<TResult>());
                }
            );
        }

        public static void RegisterAction<T>() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Function, typeof(Action<T>),
                v => {
                    var function = v.Function;
                    return (Action<T>) (p => function.Call(p));
                }
            );
        }

        public static void RegisterAction() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Function, typeof(Action),
                v => {
                    var function = v.Function;
                    return (Action) (() => function.Call());
                }
            );
        }
    }
}