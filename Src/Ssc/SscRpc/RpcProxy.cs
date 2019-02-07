using System;
using System.Collections.Generic;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssc.SscRpc {
    public class RpcProxy {
        private static readonly Logger Logger = LogManager.GetLogger<RpcProxy>(LogType.Middle);
        private static readonly Dictionary<MessageType, Func<IRpc>> _messages;

        static RpcProxy() {
            _messages = new Dictionary<MessageType, Func<IRpc>> {
                { MessageType.RpcInvoke, () => new RpcInvoke() },
                { MessageType.RpcResponse, () => new RpcResponse() }
            };
        }

        internal static void Add(MessageType messageType, Func<IRpc> func) {
            _messages.Add(messageType, func);
        }

        internal static void Remove(MessageType messageType) {
            _messages.Remove(messageType);
        }

        public static void Invoke(ulong remoteMessageId, IReadStream readStream, IPeer peer) {
            var index = (MessageType)readStream.ShiftRight<short>();
            if (!_messages.TryGetValue(index, out var func)) {
                Logger.Warn($"没有添加消息:{index}");
                return;
            }

            var message = func();
            message.DeserializeFields(readStream);
            message.Handle(remoteMessageId, peer, readStream);
        }

        public static bool Invoke(MessageType messageType, IRpc rpc, IWriteStream writeStream, IPeer peer) {
            return Invoke(messageType, rpc, writeStream, peer, null);
        }

        public static bool Invoke(MessageType messageType, IRpc rpc, IWriteStream writeStream, IPeer peer, ResponseCallback responseCallback) {
            if (peer == null) {
                Logger.Error($"{nameof(peer)} 为 null");
                return false;
            }

            if (!_messages.TryGetValue(MessageType.RpcInvoke, out var func)) {
                Logger.Error($"没有注册${nameof(RpcInvoke)}");
                return false;
            }

            rpc.SerializeFields(writeStream);
            writeStream.ShiftLeft((short)messageType);
            return peer.SendMessage(writeStream, responseCallback);
        }

        public static bool Invoke(IWriteStream writeStream, IPeer peer, ResponseCallback responseCallback) {
            if (peer == null) {
                Logger.Error($"{nameof(peer)} 为 null");
                return false;
            }

            if (!_messages.TryGetValue(MessageType.RpcInvoke, out var func)) {
                Logger.Error($"没有注册${nameof(RpcInvoke)}");
                return false;
            }

            return peer.SendMessage(writeStream, responseCallback);
        }
    }
}