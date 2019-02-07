using System;
using System.Collections.Generic;

namespace Ssc.SscPool {
    internal interface IPoolableObject : IDisposable {
        int Size { get; }

        void Reset();

        void SetPoolManager(PoolManager poolManager);
    }

    internal class PoolManager {
        private const int MaxSizePerType = 10 * (1 << 10); // 10 MB

        private readonly Dictionary<Type, Pool> pools = new Dictionary<Type, Pool>();

        public int TotalCount {
            get {
                var sum = 0;
                foreach (var pool in pools.Values) {
                    sum += pool.Count;
                }
                return sum;
            }
        }

        public T GetObject<T>() where T : class, IPoolableObject, new() {
            T valueToReturn = null;

            if (pools.TryGetValue(typeof(T), out var pool))
                if (pool.Stack.Count > 0)
                    valueToReturn = pool.Stack.Pop() as T;

            if (valueToReturn == null) valueToReturn = new T();

            valueToReturn.SetPoolManager(this);
            return valueToReturn;
        }

        public void ReturnObject<T>(T value) where T : class, IPoolableObject, new() {
            if (!pools.TryGetValue(typeof(T), out var pool)) {
                pool = new Pool();
                pools[typeof(T)] = pool;
            }

            if (value.Size + pool.PooledSize < MaxSizePerType) {
                pool.PooledSize += value.Size;
                value.Reset();
                pool.Stack.Push(value);
            }
        }

        private class Pool {
            public Pool() {
                Stack = new Stack<IPoolableObject>();
            }

            public int PooledSize { get; set; }
            public int Count => Stack.Count;
            public Stack<IPoolableObject> Stack { get; }
        }
    }

    internal class MyObject : IPoolableObject {
        private PoolManager poolManager;
        public byte[] Data { get; set; }
        public int UsableLength { get; set; }
        public int Size => Data != null ? Data.Length : 0;

        void IPoolableObject.Reset() {
            UsableLength = 0;
        }

        void IPoolableObject.SetPoolManager(PoolManager poolManager) {
            this.poolManager = poolManager;
        }

        public void Dispose() {
            poolManager.ReturnObject(this);
        }
    }
}