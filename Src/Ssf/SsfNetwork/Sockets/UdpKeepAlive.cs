using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Ssc.SscLog;

namespace Ssf.SsfNetwork.Sockets {
    internal sealed class UdpKeepAlive {
        private readonly Logger Logger = LogManager.GetLogger<UdpKeepAlive>(LogType.Low);
        private readonly AutoResetEvent _autoResetEvent;
        private readonly object _lockObject = new object();

        private readonly ConcurrentDictionary<SocketService,Tuple> _concurrentDictionary;
        private readonly CancellationTokenSource _tokenSource;

        private int _reconnectMaxCount;

        private Action _ackAction = () => { };
        private Action _disconnectAction = () => { };

        public UdpKeepAlive() {
            _autoResetEvent = new AutoResetEvent(false);
            _concurrentDictionary = new ConcurrentDictionary<SocketService, Tuple>();
            _tokenSource = new CancellationTokenSource();
        }

        public void Start(int interval, int reconnectMaxCount) {
            _reconnectMaxCount = reconnectMaxCount;
            Task.Factory.StartNew(Dispatch, interval, _tokenSource.Token);
        }

        public void Stop() {
            _tokenSource.Cancel();
        }

        public void AddAck(SocketService socketService, Action ackAction, Action disconnectAction) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)}为 null!");
                return;
            }

            if (ackAction == null) {
                Logger.Error($"{nameof(ackAction)}为 null!");
                return;
            }

            if (disconnectAction == null) {
                Logger.Error($"{nameof(disconnectAction)}为 null!");
                return;
            }

            void DisAction() {
                if (_concurrentDictionary.TryGetValue(socketService, out var result)) {
                    Logger.Info($"重置次数:{result.Count}");
                    if (result.Count >= _reconnectMaxCount) {
                        disconnectAction?.Invoke();
                        return;
                    }

                    lock (_lockObject) {
                        ++result.Count;
                    }
                }
            }

            void Action() {
                if (_concurrentDictionary.TryGetValue(socketService, out var result)) {
                    if (result.Count == 0) {
                        lock (_lockObject) {
                            _disconnectAction += result.DisAction;
                        }
                    }
                    ackAction?.Invoke();
                }
            }

            lock (_lockObject) {
                _ackAction += Action;
            }

            var tuple = new Tuple {
                Count = 0,
                AckAction = Action,
                DisAction = DisAction
            };

            if(!_concurrentDictionary.TryAdd(socketService, tuple)) {
                Logger.Warn("添加ACK失败！");
            }
        }

        public void RemoveDisconnectAction(SocketService socketService) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)}为 null!");
                return;
            }

            if (_concurrentDictionary.TryGetValue(socketService, out var result)) {
                lock (_lockObject) {
                    result.Count = 0;
                    _disconnectAction -= result.DisAction;
                }
                Logger.Debug("剩余DisDelegate:" + _disconnectAction.GetInvocationList().Length);
            } else {
                Logger.Warn($"获取{nameof(socketService)}失败！");
            }
        }

        public void Remove(SocketService socketService) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)}为 null!");
                return;
            }

            if (_concurrentDictionary.TryGetValue(socketService, out var result)) {
                lock (_lockObject) {
                    _ackAction -= result.AckAction;
                    _disconnectAction -= result.DisAction;
                }

                Logger.Debug("剩余AckDelegate:" + _ackAction.GetInvocationList().Length);
                Logger.Debug("剩余DisDelegate:" + _disconnectAction.GetInvocationList().Length);

                if(!_concurrentDictionary.TryRemove(socketService, out _)) {
                    Logger.Warn($"删除{nameof(socketService)}失败！");
                }
            } else {
                Logger.Warn($"获取{nameof(socketService)}失败！");
            }
        }

        private void Dispatch(object state) {
            var interval = (int) state;
            while (true) {
                lock (_lockObject) {
                    _ackAction?.Invoke();
                }

                _autoResetEvent.WaitOne(interval);
                lock (_lockObject) {
                    _disconnectAction?.Invoke();
                }
            }
        }

        private class Tuple : IEquatable<Tuple> {
            public int Count { get; set; }
            public Action AckAction { get; set; }
            public Action DisAction { get; set; }

            public bool Equals(Tuple other) {
                if (Count == other.Count &&
                    AckAction == other.AckAction &&
                    DisAction == other.DisAction) {
                    return true;
                }
                return false;
            }
        }
    }
}