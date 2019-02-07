using System;
using System.Collections.Generic;
using System.Threading;

namespace Ssc.SscPatterns {
    public class BufferBlock<T> {
        private readonly Queue<T> _queue;
        private readonly AutoResetEvent _autoResetEvent;

        public BufferBlock() {
            _autoResetEvent = new AutoResetEvent(false);
            _queue = new Queue<T>();
        }

        public int Count => _queue.Count;

        public void Produce(T t) {
            if (t == null) throw new ArgumentNullException(nameof(t));
            _queue.Enqueue(t);
            _autoResetEvent.Set();
        }

        public T Consume(int timeout) {
            if (_autoResetEvent.WaitOne(timeout)) return _queue.Dequeue();
            return default;
        }
    }
}