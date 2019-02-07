using System;
using System.Net;
using Ssc.SscConfiguration;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssf.SsfNetwork.Sockets {

    public abstract class SocketCore : ISocket {
        private static readonly Logger Logger = LogManager.GetLogger<SocketCore>(LogType.Low);

        public abstract event Func<SocketService, IReadStream,IWriteStream,bool> HandleAccept;

        public abstract event Action<SocketService,IReadStream> HandleConnect;

        public abstract SocketStatistics SocketStatistics { get; }

        #region Accept

        public abstract void Start(SocketConfig socketConfig);

        #endregion Accept

        #region Connect

        public abstract void Connect(SocketConfig socketConfig);

        public abstract void Connect(SocketConfig socketConfig, IWriteStream writeStream);

        #endregion Connect

        #region Disconnect

        protected abstract void OnDisconnect(SocketService socketService);

        public abstract void Disconnect(SocketService socketService);

        #endregion Disconnect

        #region Write

        public abstract bool Write(SocketService socketService,IWriteStream writeSteam);

        protected abstract bool WriteMessage(SocketService socketService, SendOption sendOption,IWriteStream writeStream);

        protected abstract bool WriteMessage(SocketService socketService, SendOption sendOption,ulong messageId, IWriteStream writeSteam);

        #endregion Write

        public virtual void Dispose(bool disposing) {
        }

        public void Dispose() {
            Dispose(true);
        }

        public abstract void ConnectionGenerator(IPEndPoint remoteAddress,
            IWriteStream writeStream,Action<SocketService> action);
    }
}