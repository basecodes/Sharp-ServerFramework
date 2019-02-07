namespace Ssc.SscAlgorithm.SscQueue {
    public class CircularQueue<T> {
        private static readonly object _lockObject = new object();

        private readonly T[] _queue;
        private int _queueHead;
        private int _queueTail;

        public CircularQueue(int length) {
            _queue = new T[length];
            _queueHead = 0;
            _queueTail = 0;
        }

        public bool IsEmpty => _queueHead == _queueTail;

        public bool Push(T t) {
            lock (_lockObject) {
                var tempTail = (_queueTail + 1) % _queue.Length;
                if (tempTail == _queueHead) return false;
                _queue[_queueTail] = t;
                _queueTail = tempTail;
                return true;
            }
        }

        public T Pop() {
            lock (_lockObject) {
                if (_queueHead == _queueTail) return default;
                var tempHead = (_queueHead + 1) % _queue.Length;
                var t = _queue[_queueHead];
                _queueHead = tempHead;
                return t;
            }
        }
    }
}