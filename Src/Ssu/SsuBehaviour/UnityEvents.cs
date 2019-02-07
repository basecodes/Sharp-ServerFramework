using System;
using Ssc.SscStream;
using UnityEngine.Events;

namespace Ssu.SsuBehaviour {
    [Serializable]
    public class Callback {
        public ConnectEvent ConnectEvent;
        public ReconnectEvent ReconnectEvent;
        public DisconnectEvent DisconnectEvent;
    }
    [Serializable]
    public class ConnectEvent : UnityEvent<bool,IReadStream> {
    }

    [Serializable]
    public class ReconnectEvent : UnityEvent<bool> {
    }

    [Serializable]
    public class DisconnectEvent : UnityEvent {
    }
}
