using System.Collections.Generic;

namespace Ssc.SscPatterns {
    public class EventManager<TEventType, TEventSource> {
        public delegate void Handle(TEventSource sender, params object[] Params);

        private Dictionary<TEventType, Handle> _listeners;

        public EventManager() {
            _listeners = new Dictionary<TEventType, Handle>();
        }

        //注册
        public void AddListener(TEventType eventType, Handle listener) {
            if (_listeners.TryGetValue(eventType, out var temp)) return;

            _listeners?.Add(eventType, listener);
        }

        //通知
        public bool PostNotification(TEventType eventType,
            TEventSource sender, params object[] args) {
            if (!_listeners.TryGetValue(eventType, out var listen)) return false;

            listen.Invoke(sender, args);
            return true;
        }

        public void RemoveRedundancies() {
            var tmpListeners = new Dictionary<TEventType, Handle>();
            foreach (var Item in _listeners)
                if (Item.Value != null)
                    tmpListeners.Add(Item.Key, Item.Value);
            _listeners = tmpListeners;
        }

        public void Clear() {
            _listeners.Clear();
        }

        public void RemoveEvent(TEventType eventType) {
            _listeners.Remove(eventType);
        }
    }
}