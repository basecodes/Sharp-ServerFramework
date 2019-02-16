using System;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssf.SsfNetwork.Sockets;
using Ssm.SsmComponent;

namespace Ssf.SsfNetwork.Peers {
    internal class Peer : BasePeer {

        private static readonly Logger Logger = LogManager.GetLogger<Peer>(LogType.Middle);

        public override ConnectionStatus Status { get; protected set; }
        public override Connection Connection => _socketService.Connection;

        private SocketService _socketService;
        private IServerSocket _serverSocket;
        
        public override void Disconnect() {
            _serverSocket.Socket.Disconnect(_socketService);
        }

        public void SetStatus(ConnectionStatus status, SocketService socketService,IServerSocket serverSocket) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null");
                return;
            }

            if (status != ConnectionStatus.Connected) {
                Logger.Debug("Disconnected!");
            }

            Status = status;
            if (status == ConnectionStatus.Connected) {
                _socketService = socketService;
            }

            _serverSocket = serverSocket ?? throw new ArgumentNullException(nameof(serverSocket));
        }

        public override bool SendMessage(ulong id,IWriteStream writeStream) {
            if (Status != ConnectionStatus.Connected) {
                Logger.Warn($"ID：{ID}未连接状态！");
                return false;
            }

            if (_socketService == null) {
                Logger.Error($"{nameof(_socketService)} 为 null");
                return false;
            }

            var securityComponent = GetComponent<ISecurityComponent>();
            if (securityComponent == null) {
                Logger.Error($"{nameof(securityComponent)} 为 null");
                return false;
            }

            Ssfi.Security.EncryptAES(writeStream, securityComponent.AesKey);
            writeStream.ShiftLeft(id);
            return _serverSocket.Socket.Write(_socketService, writeStream);
        }



        public override void Recycle() {
            base.Recycle();
            Status = ConnectionStatus.None;
            _socketService = null;
            _serverSocket = null;
        }
    }
}