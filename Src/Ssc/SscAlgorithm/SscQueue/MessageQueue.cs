using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Ssc.SscAlgorithm.SscQueue {
    public class MessageQueue {
        private readonly AutoResetEvent _autoResetEvent;
        private readonly ConcurrentQueue<Action> _queue;
        private Task _currentProcessTask;
        private bool _isRunning;

        public MessageQueue() {
            _autoResetEvent = new AutoResetEvent(false);
            _queue = new ConcurrentQueue<Action>();
        }

        public void Start() {
            _isRunning = true;
            _currentProcessTask = Task.Factory.StartNew(Dispatch);
        }

        public void Stop() {
            _isRunning = false;

            try {
                _currentProcessTask?.Wait();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public void Enqueue(Action message) {
            if (message == null) throw new ArgumentNullException(nameof(message));
            _queue.Enqueue(message);
            _autoResetEvent.Set();
        }

        private Action Dequeue() {
            _queue.TryDequeue(out var result);
            return result;
        }

        private void Dispatch() {
            while (_isRunning) {
                var action = Dequeue();
                if (action == null)
                    _autoResetEvent.WaitOne();
                else
                    try {
                        action?.Invoke();
                    } catch (Exception e) {
                        Console.WriteLine(e);
                    }
            }
        }
    }
}