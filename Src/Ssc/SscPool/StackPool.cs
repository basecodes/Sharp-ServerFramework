using System;
using System.Collections.Generic;

namespace Ssc.SscPool {
    public class StackPool<T> where T : new() {
        protected int _maxCapacity;
        protected Stack<T> _pool;

        public StackPool(int maxCapacity) {
            _maxCapacity = maxCapacity;
            _pool = new Stack<T>(_maxCapacity);
            for (var i = 0; i < maxCapacity; ++i) _pool.Push(new T());
        }

        public int Count => _pool.Count;

        public T Pop() {
            lock (_pool) {
                if (_pool.Count == 0) throw new StackOverflowException();
                return _pool.Pop();
            }
        }

        public void Push(T item) {
            lock (_pool) {
                if (item == null) throw new ArgumentNullException(nameof(item));
                _pool.Push(item);
            }
        }

        public void Clear() {
            _pool.Clear();
        }
    }
}