using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Ssm.SsmCache;
using Sss.SssScripts.Lua;

namespace Ssf.SsfCache {
    public class LuaCache:ICache {
        public Table Instance { get; }
        private readonly LuaHelper _luaHelper;

        public LuaCache(Table instance, LuaHelper luaHelper) {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _luaHelper = luaHelper ?? throw new ArgumentNullException(nameof(luaHelper));
        }

        public DynValue GetField(string name) {
            return Instance.Get(name);
        }

        public DynValue Call(string methodName, params object[] args) {
            var list = new List<object> {Instance};
            list.AddRange(args);
            return _luaHelper.Call(Instance, methodName, list.ToArray());
        }
    }
}