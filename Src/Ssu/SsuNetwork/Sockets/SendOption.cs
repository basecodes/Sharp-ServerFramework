using System;

namespace Ssu.SsuNetwork.Sockets {
    [Flags]
    internal enum SendOption:byte {
        Acknowledge = 0x1,
        Connect = 0x2,
        Fragment = 0x4,
        Disconnect = 0x8,
        Ack = 0x10
    }
}