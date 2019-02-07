using System;
using System.Linq;
using Ssc.Ssc;
using Ssc.SscAlgorithm.SscQueue;
using Ssc.SscConfiguration;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssf.SsfNetwork.Peers;
using Ssf.SsfNetwork.Sockets;
using Ssm.Ssm;
using Ssm.SsmComponent;

namespace Ssf.SsfNetwork {
    internal class ServerSocket : IServerSocket {
        private static readonly Logger Logger = LogManager.GetLogger<ServerSocket>(LogType.Middle);

        public ISocket Socket { get; }

        public event Action<IUser, IReadStream> Connected;
        public event Func<IUser, IReadStream, IWriteStream, bool> Accepted;
        public event Action<IUser> Disconnected;
        private MessageQueue _messageQueue;
        private string _serviceId;
        public ServerSocket(string serviceId) {
            if (string.IsNullOrEmpty(serviceId)) {
                throw new ArgumentException(nameof(serviceId));
            }

            Socket = new APM_UdpSocket(sizeof(ulong));
            _serviceId = serviceId;
            PoolAllocator<IPeer>.SetPool(args =>
                ObjectFactory.GetActivator<IPeer>(typeof(Peer).GetConstructors().First())());

            Socket.HandleAccept += Accept;
            Socket.HandleConnect += PeerConnected;

            _messageQueue = new MessageQueue();
        }

        public void Start(SocketConfig serverConfig) {
            if (serverConfig == null) {
                throw new ArgumentNullException(nameof(serverConfig));
            }
            _messageQueue.Start();
            Socket.Start(serverConfig);
        }

        public void Stop() {
            Socket.Dispose(true);

            Connected = null;
            Disconnected = null;
        }

        private bool Accept(SocketService socketService, IReadStream readStream,IWriteStream writeStream) {
            var peer = AddListener(socketService);
            return Accepted == null ? true:Accepted.Invoke(peer, readStream, writeStream);
        }
        
        private void PeerConnected(SocketService socketService, IReadStream readStream) {
            var peer = AddListener(socketService);
            Connected?.Invoke(peer, readStream);
        }

        public IUser AddListener(SocketService socketService) {
            var peer = PoolAllocator<IPeer>.GetObject() as Peer;
            peer.SetStatus(ConnectionStatus.Connected, socketService,this);

            socketService.HandleDisconnect += () => {
                if (peer.Status != ConnectionStatus.Connected) {
                    return;
                }
                Disconnected?.Invoke(peer);
                peer.Recycle();
            };

            socketService.HandleRead += (readStream) => {
                var tmp = readStream.Clone();
                _messageQueue.Enqueue(() => {
                    var securityComponent = peer.GetComponent<ISecurityComponent>();
                    if (securityComponent == null) {
                        Logger.Error($"{nameof(securityComponent)}组件为空！");
                        return;
                    }

                    var remoteMessageId = tmp.ShiftRight<ulong>();
                    Ssfi.Security.DecryptAES(tmp, securityComponent.AesKey);
                    RpcProxy.Invoke(remoteMessageId, tmp, peer);
                    tmp.Dispose();
                });
            };

            socketService.HandleError += reason => Logger.Error(reason);
            socketService.HandleWrite += (isSuccess) => {
                if (!isSuccess)
                    Logger.Error($"消息发送失败！");
            };

            socketService.HandleAcknowledge += (isSuccess, readStream) => {
                var securityComponent = peer.GetComponent<ISecurityComponent>();
                if (securityComponent == null) {
                    Logger.Error($"{nameof(securityComponent)}组件为空！");
                    return;
                }

                // local
                var localMessageId = readStream.ShiftRight<ulong>();
                peer.Acknowledge(isSuccess, localMessageId);
            };
            return peer;
        }

        public void Connect(SocketConfig socketConfig) {
            if (socketConfig == null) {
                throw new ArgumentNullException(nameof(socketConfig));
            }

            _messageQueue.Start();
            Socket.Connect(socketConfig);
        }

        public void Connect(SocketConfig socketConfig, IWriteStream writeStream) {
            if (socketConfig == null) {
                throw new ArgumentNullException(nameof(socketConfig));
            }

            if (writeStream == null) {
                throw new ArgumentNullException(nameof(writeStream));
            }

            _messageQueue.Start();
            Socket.Connect(socketConfig, writeStream);
        }
    }
}