using System;
using System.Collections.Concurrent;
using Ssc.SscSerialization;
using Ssm.Ssm;
using Sss.SssScripts.Lua;

namespace Ssf.SsfManager {
    public class RawMessageManager {
        private readonly ConcurrentDictionary<ushort, Action<IUser, IDeserializable>> _rawMessages;

        public RawMessageManager() {
            _rawMessages = new ConcurrentDictionary<ushort, Action<IUser, IDeserializable>>();
            LuaHelper.RegisterType<RawMessageManager>();
        }

        public void AddRawMessage(ushort opCode, Action<IUser, IDeserializable> action) {
            _rawMessages.TryAdd(opCode, action);
        }

        public Action<IUser, IDeserializable> GetRawMessageHandler(ushort opCode) {
            _rawMessages.TryGetValue(opCode, out var value);
            return value;
        }

        public void RemoveRawMessage(ushort opCode) {
            _rawMessages.TryRemove(opCode, out var value);
        }
    }
}