using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Ssc.SscLog;

namespace Ssf.SsfNetwork.Sockets {
    internal sealed class FragmentsTimer {
        
        private readonly AutoResetEvent _autoResetEvent;
        private Action _action;
        private readonly object _lockObject = new object();
        private readonly Logger Logger = LogManager.GetLogger<FragmentsTimer>(LogType.Low);
        private readonly ConcurrentDictionary<string,Tuple> _resend;

        private readonly CancellationTokenSource _tokenSource;
        
        public FragmentsTimer() {
            _action = () => { };
            _tokenSource = new CancellationTokenSource();
            _autoResetEvent = new AutoResetEvent(false);
            _resend = new ConcurrentDictionary<string, Tuple>();
        }

        public void Start(int interval) {
            Task.Factory.StartNew(Dispatch, interval, _tokenSource.Token);
        }

        public void Stop() {
            _tokenSource.Cancel();
        }

        public bool Add(string id, Action action, Action destroyAction) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }

            if (destroyAction == null) {
                throw new ArgumentNullException(nameof(destroyAction));
            }

            if (_resend.ContainsKey(id)) {
                Logger.Warn($"重发ID:{id}已经添加!");
                return false;
            }

            if (!_resend.TryAdd(id, new Tuple {Action = action, DestroyAction = destroyAction})) {
                return false;
            }
            
            lock (_lockObject) {
                _action += action;
            }
            return true;

        }

        public bool Remove(string id) {
            if (!_resend.TryRemove(id, out var value)) {
                Logger.Warn($"移除ID:{id}失败!");
                return false;
            }
            
            lock (_lockObject) {
                _action -= value.Action;
            }
            value.DestroyAction?.Invoke();
            return true;
        }

        private void Dispatch(object state) {
            var interval = (int) state;
            while (true) {
                lock (_lockObject) {
                    _action?.Invoke();
                }

                _autoResetEvent.WaitOne(interval);
            }
        }

        private struct Tuple {
            public Action Action { get; set; }
            public Action DestroyAction { get; set; }
        }
    }
}