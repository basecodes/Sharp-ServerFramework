using System;
using System.Collections.Generic;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssm.Ssm;
using Ssm.SsmComponent;

namespace Ssf.SsfNetwork.Peers {
    internal abstract class BasePeer : PoolAllocator<IPeer>,IUser {
        private static readonly Logger Logger = LogManager.GetLogger<BasePeer>(LogType.Middle);

        private readonly object _lockAcks = new object();
        private readonly object _lockCallbacks = new object();

        private readonly Dictionary<ulong, ResponseCallback> _acks;
        private readonly Dictionary<ulong, Action> _callback;
        private readonly Dictionary<string, IPeerComponent> _components;

        public Guid ID { get; private set; }
        
        public abstract ConnectionStatus Status { get; protected set; }
        public abstract Connection Connection { get; }

        Ssc.Ssc.ConnectionStatus IPeer.Status => throw new NotImplementedException();


        private ulong _id;
        protected BasePeer() {
            _components = new Dictionary<string, IPeerComponent>();
            _acks = new Dictionary<ulong, ResponseCallback>();
            _callback = new Dictionary<ulong, Action>();
            ID = Guid.NewGuid();
        }
       
        public abstract void Disconnect();

        public abstract bool SendMessage(ulong id, IWriteStream writeStream);

        public bool SendMessage(IWriteStream writeStream,ResponseCallback responseCallback) {
            if (Status != ConnectionStatus.Connected) {
                Logger.Warn($"ID:{ID}没有连接");
                return false;
            }

            if (responseCallback == null) {
                return SendMessage(0, writeStream);
            }

            lock (_lockAcks) {
                _id++;
                _acks.Add(_id, responseCallback);
            }
            return SendMessage(_id, writeStream);
        }

        public void AddComponent<T>(T component) where T : class, IPeerComponent {
            AddComponent(typeof(T).Name, component);
        }

        public void AddComponent(string componentName, IPeerComponent component) {
            if (!HasComponent(componentName)){
                _components.Add(componentName, component);
            }else{
                Logger.Warn($"类型{componentName}组件已经添加！");
            }
        }

        public void RemoveComponent<T>() where T : class, IPeerComponent {
            RemoveComponent(typeof(T).Name);
        }

        public void RemoveComponent(string componentName) {
            if (HasComponent(componentName))
                _components.Remove(componentName);
            else
                Logger.Warn($"类型{componentName}组件移除失败！");
        }

        public T GetComponent<T>() where T : class, IPeerComponent {
            return GetComponent(typeof(T).Name) as T;
        }

        public T GetComponent<T>(string componentName)where T : class, IPeerComponent {
            return GetComponent(componentName) as T;
        }

        public IPeerComponent GetComponent(string componentName) {
            _components.TryGetValue(componentName, out var component);
            return component;
        }

        public bool HasComponent(string componentName) {
            return _components.ContainsKey(componentName);
        }

        public bool HasComponent<T>() where T : class, IPeerComponent {
            return HasComponent(typeof(T).Name);
        }

        public void Response(ulong messageId, IResponseMessage responseMessage, IDeserializable singleDeserializable) {
            if (_acks.TryGetValue(messageId,out var responseCallback)) {
                var success = false;
                lock (_lockAcks) {
                    success = _acks.Remove(messageId);
                }

                if (success)
                    responseCallback?.Invoke(responseMessage, singleDeserializable);
                else
                    Logger.Warn($"删除{nameof(messageId)} = {messageId}失败！");
            }
        }

        public void Acknowledge(bool result, ulong messageId) {
            if (_callback.TryGetValue(messageId, out var acknowledge)) {
                var success = false;
                lock (_lockCallbacks) {
                    success = _callback.Remove(messageId);
                }

                if (success && result) {
                    try {
                        acknowledge?.Invoke();
                    } catch (Exception e) {
                        Logger.Error(e);
                    }
                }
            }
        }

        public void Dispose() {
            Recycle(this);
        }

        public virtual void Recycle() {
            _acks.Clear();
            _callback.Clear();
            _components.Clear();
            ID = Guid.NewGuid();
        }

        public void Assign() {
        }
    }
}