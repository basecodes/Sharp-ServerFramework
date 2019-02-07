using System.Net;
using Ssc.Ssc;
using Ssc.SscStream;
using Ssu.SsuNetwork.Sockets;
using Ssu;

namespace Ssu.SsuNetwork.Peers {
    internal class Peer : BasePeer {
        private static readonly Ssc.SscLog.Logger Logger = Ssc.SscLog.LogManager.GetLogger<Peer>(Ssc.SscLog.LogType.Middle);

        private ConnectionStatus _connectionStatus;
        private readonly ISocket _socket;
        public SocketService SocketService { get; private set; }
        public override ConnectionStatus Status => _connectionStatus;
        public override Connection Connection => SocketService.Connection;
        private string _aesKey;
        public Peer(ISocket socket,string aesKey) {
            _socket = socket;
            _aesKey = aesKey;
        }

        public override void Disconnect() {
            _connectionStatus = ConnectionStatus.Disconnected;
            _socket.Disconnect(SocketService);
        }

        public void SetStatus(ConnectionStatus status, SocketService socketService) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null");
                return;
            }

            if (status != ConnectionStatus.Connected) {
                Logger.Debug("Disconnected!");
            }

            _connectionStatus = status;
            if (status == ConnectionStatus.Connected) {
                SocketService = socketService;
            }
        }

        public override bool SendMessage(ulong id, IWriteStream writeStream) {
            if (Status != ConnectionStatus.Connected) {
                Logger.Warn("没有连接！");
                return false;
            }

            Logger.Debug("Key:" + _aesKey);
            Ssui.Security.EncryptAES(writeStream,_aesKey);
            writeStream.ShiftLeft(id);
            return _socket.Write(SocketService, writeStream);
        }
    }
}