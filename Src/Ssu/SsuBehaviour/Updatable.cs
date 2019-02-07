using System;
using System.Collections.Concurrent;

namespace Ssu.SsuBehaviour {
    public interface IUpdatable {
        void Update();
    }

    public abstract class Updatable : IUpdatable {
        private readonly ConcurrentQueue<Action> _readQueue;

        protected Updatable() {
            _readQueue = new ConcurrentQueue<Action>();
        }

        public virtual void Update() {
            Dequeue()?.Invoke();
        }

        protected void Enqueue(Action action) {
            _readQueue.Enqueue(action);
        }

        protected Action Dequeue() {
             _readQueue.TryDequeue(out var result);
            return result;
        }
    }
}