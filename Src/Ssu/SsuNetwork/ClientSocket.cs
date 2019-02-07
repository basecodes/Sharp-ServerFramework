using System;
using System.Net;
using System.Threading.Tasks;
using Ssc;
using Ssc.Ssc;
using Ssc.SscExtension;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssu.Ssu;
using Ssu.SsuBehaviour;
using Ssu.SsuNetwork.Peers;
using Ssu.SsuNetwork.Sockets;
using Ssu.SsuSecurity;

namespace Ssu.SsuNetwork {
    internal class ClientSocket : Updatable, IClientSocket {

        public event Action<IPeer, IReadStream> Connected;
        public event Action<IPeer> Disconnected;

        private static readonly Logger Logger = LogManager.GetLogger<ClientSocket>(LogType.Middle);
        private readonly ISocket _socket;
        private Keys _keys;
        public ClientSocket(NetworkConfig networkConfig,UpdateRunner updateRunner) {
            if (networkConfig == null) {
                throw new ArgumentNullException(nameof(networkConfig));
            }

            if (updateRunner == null) {
                throw new ArgumentNullException(nameof(updateRunner));
            }

            updateRunner.Add(this);

            _socket = new UdpSocket(sizeof(ulong),networkConfig);
            _socket.HandleConnect += (socketService, rs) => {
                if (rs == null) {
                    Logger.Warn("连接失败！");
                    Enqueue(() => {
                        Connected?.Invoke(null, null);
                    });
                    return;
                }

                var length = rs.ShiftRight<ushort>();
                var data = rs.ShiftRight(length);
                var bytes = new byte[data.Count];
                Buffer.BlockCopy(data.Buffer, data.Offset, bytes, 0, bytes.Length);

                var buffer = Ssui.Security.DecryptAesKey(bytes, _keys);
                var aesKey = buffer.toString();
                Logger.Debug(aesKey);

                var peer = new Peer(_socket, aesKey);

                socketService.HandleDisconnect += () => {
                    Enqueue(() => {
                        Disconnected?.Invoke(peer);
                    });
                };

                socketService.HandleRead += (readStream) => {
                    // remote
                    var remoteMessageId = readStream.ShiftRight<ulong>();
                    var newReadStream = readStream.Clone();
                    Ssui.Security.DecryptAES(newReadStream, aesKey);
                    Enqueue(() => {
                        RpcProxy.Invoke(remoteMessageId,newReadStream, peer);
                        newReadStream.Dispose();
                    });
                };

                socketService.HandleAcknowledge += (state,readStream) => {
                    var newReadStream = readStream.Clone();

                    var localMessageId = readStream.ShiftRight<ulong>();
                    Enqueue(() => {
                        peer.Acknowledge(state, localMessageId);
                        newReadStream.Dispose();
                    } );
                };
                socketService.HandleError += error =>  Enqueue(() => Logger.Error(error));
                socketService.HandleWrite += (isSuccess) => {
                    Enqueue(() => {
                        if (!isSuccess) {
                            Logger.Warn("发送失败！");
                        }
                    });
                };

                var newRs = rs.Clone();
                Enqueue(() => {
                    peer.SetStatus(ConnectionStatus.Connected,socketService);
                    Connected?.Invoke(peer,newRs);
                    newRs.Dispose();
                });
                
            };
        }

        public void Reconnect(Action<bool> action) {
            _socket.HandleReconnect += (socketService,state) => {
                Enqueue(() => {
                    action?.Invoke(state);
                });
            };
        }

        public void Connect(string ip,int port) {
            _keys = Ssui.Security.CreateAesKey();
            var publicKey = _keys.PublicKey;
            Task.Factory.StartNew(() => {
                using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                    writeStream.ShiftRight(publicKey);
                    _socket.Connect(ip,port,writeStream);
                }
            });
        }

        private IPAddress Parse(string ipString) {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ipString, out ipAddress))
                return ipAddress;
            Logger.Error($"解析地址{ipString}出错！");
            return null;
        }

        public void Dispose() {
            _socket.Dispose();
        }
    }
}