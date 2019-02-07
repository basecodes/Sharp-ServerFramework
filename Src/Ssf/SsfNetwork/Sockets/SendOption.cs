using System;

namespace Ssf.SsfNetwork.Sockets {
    [Flags]
    public enum SendOption : byte {
        Acknowledge = 0b0000_0001,
        Connect = 0b0000_0010,
        Fragment = 0b0000_0100,
        Disconnect = 0b0000_1000,
        Ack = 0b0001_0000
    }
}