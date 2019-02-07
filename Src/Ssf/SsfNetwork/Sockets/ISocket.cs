using System;
using System.Net;
using Ssc.SscConfiguration;
using Ssc.SscStream;

namespace Ssf.SsfNetwork.Sockets {
    public interface ISocket : IDisposable {
        SocketStatistics SocketStatistics { get; }

        event Func<SocketService, IReadStream,IWriteStream,bool> HandleAccept;

        event Action<SocketService, IReadStream> HandleConnect;

        bool Write(SocketService socketEvent,IWriteStream writeStream);

        void ConnectionGenerator(IPEndPoint remoteAddress,IWriteStream writeStream,Action<SocketService> action);
        void Start(SocketConfig socketConfig);

        void Connect(SocketConfig socketConfig);

        void Connect(SocketConfig socketConfig, IWriteStream writeStream);

        void Disconnect(SocketService socketService);

        void Dispose(bool disposing);
    }
}