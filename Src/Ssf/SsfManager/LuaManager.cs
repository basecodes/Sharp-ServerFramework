using System;
using System.Collections.Concurrent;
using Ssc.SscLog;

namespace Ssf.SsfManager {
    public class LuaManager {
        private static readonly Logger Logger = LogManager.GetLogger<LuaManager>(LogType.Middle);
        private readonly ConcurrentDictionary<string, Type> _rpcObjectTypes;

        public LuaManager() {
            _rpcObjectTypes = new ConcurrentDictionary<string, Type>();
        }

        public void Register(string interfaceName, Type typeImplement) {
            if (_rpcObjectTypes.ContainsKey(interfaceName)) {
                Logger.Warn($"已经注册过{interfaceName}无需重复注册！");
                return;
            }

            _rpcObjectTypes[interfaceName] = typeImplement;
        }

        public bool Cantains(string name) {
            return _rpcObjectTypes.ContainsKey(name);
        }

        public Type GetRpcType(string name) {
            _rpcObjectTypes.TryGetValue(name, out var type);
            return type;
        }
    }
}