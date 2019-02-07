using System;
using Ssc.SscConfiguration;
using Ssc.SscStream;
using Ssf.SsfNetwork.Sockets;
using Ssm.Ssm;

namespace Ssf.SsfNetwork {
    internal interface IServerSocket {
        event Action<IUser,IReadStream> Connected;
        event Func<IUser,IReadStream,IWriteStream,bool> Accepted;
        event Action<IUser> Disconnected;

        ISocket Socket { get; }
        
        IUser AddListener(SocketService socketService);
        void Connect(SocketConfig socketConfig);
        void Connect(SocketConfig socketConfig, IWriteStream writeStream);
        void Start(SocketConfig socketConfig);
        void Stop();
    }
}