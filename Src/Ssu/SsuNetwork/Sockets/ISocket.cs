using System;
using Ssc.SscStream;

namespace Ssu.SsuNetwork.Sockets {

    public interface ISocket:IDisposable {
        event Action<SocketService, IReadStream> HandleConnect;
        event Action<SocketService,bool> HandleReconnect;

        void Connect(string ip,int port);
        void Connect(string ip,int port,IWriteStream writeStream);

        bool Write(SocketService socketService,IWriteStream writeStream);

        void Disconnect(SocketService socketService);
    }
}