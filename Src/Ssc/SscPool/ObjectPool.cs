using System;
using System.Collections.Concurrent;

namespace Ssc.SscPool {
    public class ObjectPool<T> {
        private readonly ConcurrentBag<T> _objects;
        private readonly ObjectActivator<T> _objectGenerator;

        public ObjectPool(ObjectActivator<T> objectGenerator) {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objects = new ConcurrentBag<T>();
        }

        public void PutObject(T item) {
            _objects.Add(item);
        }

        public T GetObject(params object[] args) {
            if (_objects.TryTake(out var item)) {
                return item;
            }

            return _objectGenerator(args);
        }
    }
}