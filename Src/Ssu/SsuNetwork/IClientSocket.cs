using System;
using Ssc.Ssc;
using Ssc.SscStream;

namespace Ssu.SsuNetwork {

    internal interface IClientSocket:IDisposable {

        event Action<IPeer, IReadStream> Connected;

        event Action<IPeer> Disconnected;

        void Connect(string ip,int port);

    }
}