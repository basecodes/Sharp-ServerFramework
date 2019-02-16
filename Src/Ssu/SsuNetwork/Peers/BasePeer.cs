using System;
using System.Collections.Concurrent;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;

namespace Ssu.SsuNetwork.Peers {
    public abstract class BasePeer :PoolAllocator<IPeer>, IPeer {
        private static readonly Logger Logger = LogManager.GetLogger<BasePeer>(LogType.Middle);
        private readonly ConcurrentDictionary<ulong, ResponseCallback> _acks;
        private readonly ConcurrentDictionary<ulong, Action> _callback;

        public Guid ID { get; private set; }
        public abstract ConnectionStatus Status { get; }
        public abstract Connection Connection { get; }

        public abstract void Disconnect();
        public abstract bool SendMessage(ulong id, IWriteStream writeStream);

        private ulong _id;
        protected BasePeer() {
            _acks = new ConcurrentDictionary<ulong, ResponseCallback>();
            _callback = new ConcurrentDictionary<ulong, Action>();
            ID = Guid.NewGuid();
        }

        public bool SendMessage(IWriteStream writeStream,ResponseCallback responseCallback){
            if (Status != ConnectionStatus.Connected) {
                Logger.Warn("没有连接！");
                return false;
            }

            if (responseCallback == null) {
                return SendMessage(0,writeStream);
            }

            _acks[++_id] = responseCallback;
            return SendMessage(_id,writeStream);
        }

        public void Response(ulong messageId, IResponseMessage responseMessage,
            IDeserializable singleDeserializable) {
            if (messageId == 0) {
                return;
            }

            foreach (var item in _acks) {
                Logger.Debug(item.Key);
            }
            if (_acks.TryRemove(messageId, out var responseCallback)){
                responseCallback?.Invoke(responseMessage, singleDeserializable);
            }else{
                Logger.Error($"删除 {nameof(messageId)} = {messageId}失败！");
            }
        }

        public void Dispose() {
            Recycle(this);
        }

        public void Recycle() {
            _acks.Clear();
            ID = Guid.NewGuid();
        }

        public void Acknowledge(bool success, ulong messageId) {
            if (_callback.TryRemove(messageId, out var action)) {
                if (success) {
                    action?.Invoke();
                }
            }
        }

        public void AddCallback(ulong messageId, Action callback) {
            if (callback != null) {
                if (_callback.TryGetValue(messageId, out var action)) {
                    var temp = action + callback;
                    _callback.TryUpdate(messageId, temp, action);
                } else {
                    _callback.TryAdd(messageId, callback);
                }
            }
        }

        public void Assign() {
        }
    }
}