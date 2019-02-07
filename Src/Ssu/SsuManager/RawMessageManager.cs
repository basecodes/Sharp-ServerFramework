using System;
using System.Collections.Generic;
using Ssc.Ssc;
using Ssc.SscSerialization;

namespace Ssu.SsuManager {
    internal class RawMessageManager {
        private static readonly Dictionary<ushort, Action<IPeer, ISerializable>> _rawMessages;

        static RawMessageManager() {
            _rawMessages = new Dictionary<ushort, Action<IPeer, ISerializable>>();
        }

        public static void AddRawMessage(ushort opCode, Action<IPeer, ISerializable> action) {
            _rawMessages.Add(opCode, action);
        }

        public static Action<IPeer, ISerializable> GetRawMessageHandler(ushort opCode) {
            Action<IPeer, ISerializable> value;
            _rawMessages.TryGetValue(opCode, out value);
            return value;
        }

        public static void RemoveRawMessage(ushort opCode) {
            _rawMessages.Remove(opCode);
        }
    }
}