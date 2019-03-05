using System;
using Ssc.SscAlgorithm.SscQueue;
using Ssc.SscConfiguration;
using Ssc.SscStream;
using Ssf.SsfNetwork.Sockets;
using Ssm.Ssm;

namespace Ssf.SsfNetwork {
    internal interface IServerSocket {
        event Action<IUser,IReadStream> Connected;
        event Action<IUser,IReadStream,IWriteStream> Accepted;
        event Action<IUser> Disconnected;

        ISocket Socket { get; }

        void AddEvent(Action evt);
        IUser AddListener(SocketService socketService);
        void Connect(SocketConfig socketConfig);
        void Connect(SocketConfig socketConfig, IWriteStream writeStream);
        void Start(SocketConfig socketConfig);
        void Stop();
    }
}