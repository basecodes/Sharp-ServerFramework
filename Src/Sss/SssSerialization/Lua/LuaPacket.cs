using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Ssc.Ssc;
using Ssc.SscSerialization;
using Sss.SssScripts.Lua;

namespace Sss.SssSerialization.Lua {
    public interface ILuaPacket : ISerializablePacket {
        Table Instance { get; }
    }

    public class LuaPacket : ScriptPoolAllocator<ILuaPacket>, ILuaPacket {
        private readonly LuaHelper _luaHelper;
        public override string TypeName { get; }
        public Table Instance { get; }
        public LuaPacket(string interfaceName,Table instance, LuaHelper luaHelper) {
            TypeName = interfaceName ?? throw new ArgumentNullException(nameof(interfaceName));
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _luaHelper = luaHelper ?? throw new ArgumentNullException(nameof(luaHelper));
        }

        private DynValue Call(string methodName, params object[] args) {
            var list = new List<object>();
            list.Add(Instance);
            list.AddRange(args);
            return _luaHelper.Call(Instance, methodName, list.ToArray());
        }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            Call("FromBinaryReader", reader);
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            Call("ToBinaryWriter", writer);
        }

        public void Recycle() {
            Call("Recycle");
        }

        public void Assign() {
            Call("Assign");
        }

        public Table GetObject(string interfaceName) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            return ScriptPoolAllocator<ILuaPacket>.GetObject(interfaceName).Instance;
        }
    }
}