using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Ssc.Ssc;
using Ssc.SscSerialization;
using Sss.SssScripts.Lua;

namespace Sss.SssSerialization {
    internal interface ILuaPacket : IRecyclable {
        Table Instance { get; }
    }

    internal class LuaPacket : LuaSerializablePacket<ILuaPacket>, ILuaPacket {
        private readonly LuaHelper _luaHelper;
        public Table Instance { get; }
        public LuaPacket(Table instance, LuaHelper luaHelper) {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _luaHelper = luaHelper ?? throw new ArgumentNullException(nameof(luaHelper));
        }

        private DynValue Call(string methodName, params object[] args) {
            var list = new List<object>();
            list.Add(Instance);
            list.AddRange(args);
            return _luaHelper.Call(Instance, methodName, list.ToArray());
        }

        public override void FromBinaryReader(IEndianBinaryReader reader) {
            Call("FromBinaryReader", reader);
        }

        public override void ToBinaryWriter(IEndianBinaryWriter writer) {
            Call("ToBinaryWriter", writer);
        }

        public void Recycle() {
            Call("Dispose");
        }
    }
}