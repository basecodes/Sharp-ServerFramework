using System;
using MoonSharp.Interpreter;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssm.Ssm;

namespace Sss.SssRpc {
    internal sealed class LuaController : Controller {
        private static readonly Logger Logger = LogManager.GetLogger<LuaController>(LogType.Middle);

        public Table Instance { get; }

        public LuaController(Table table) {
            Instance = table ?? throw new ArgumentNullException(nameof(table));
        }
    }
}