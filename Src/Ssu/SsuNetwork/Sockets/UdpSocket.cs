using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ssc.SscExtension;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssu.Ssu;
using Ssu.SsuNetwork.DataPacks;

namespace Ssu.SsuNetwork.Sockets {
    public sealed class UdpSocket : ISocket {
        public event Action<SocketService, IReadStream> HandleConnect;
        public event Action<SocketService,bool> HandleReconnect;
        
        private static readonly Logger Logger = LogManager.GetLogger<UdpSocket>(LogType.Low);
        private readonly CancellationTokenSource _tokenSource;
        private readonly ConcurrentDictionary<EndPoint, SocketService> _connections;

        private readonly UdpClient _writer;

        private readonly AutoResetEvent _autoResetEvent;
        private readonly NetworkConfig _networkConfig;
        private readonly FragmentsTimer _fragmentsTimer;
        private const int MTU = 508;
        private int _nextHeadOffset;
        public UdpSocket(int nextHeadOffset,NetworkConfig networkConfig) {
            _networkConfig = networkConfig ?? throw new ArgumentNullException(nameof(networkConfig));
            _tokenSource = new CancellationTokenSource();
            _autoResetEvent = new AutoResetEvent(false);
            _connections = new ConcurrentDictionary<EndPoint, SocketService>();
            _fragmentsTimer = new FragmentsTimer();

            _nextHeadOffset = nextHeadOffset;

            PoolAllocator<IWriteStream>.SetPool(args =>
                ObjectFactory.GetActivator<IWriteStream>(typeof(WriteStream).GetConstructors().First())());

            PoolAllocator<IReadStream>.SetPool(args =>
                ObjectFactory.GetActivator<IReadStream>(typeof(ReadStream).GetConstructors().First())());

            if (_networkConfig.KeepAlive) {
                UdpKeepAlive.Start(_networkConfig.AckInterval, _networkConfig.MaxReconnectCount);
            }

            if (_networkConfig.FragmentResend) {
                _fragmentsTimer.Start(_networkConfig.ResendInterval);
            }

            var ipAddress = new IPEndPoint(IPAddress.Parse(_networkConfig.ServerIp), _networkConfig.ServerPort);

            _writer = new UdpClient(ipAddress.AddressFamily) {
                Client = CreateSocket(ipAddress.AddressFamily, null)
            };

            Task.Factory.StartNew(ReceiveAsync, _networkConfig, _tokenSource.Token);
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect(string ip,int port) {
            using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                Connect(ip,port,writeStream);
            }
        }

        /// <summary>
        /// 带数据的连接
        /// </summary>
        public void Connect(string ip,int port,IWriteStream writeStream) {
            if (writeStream == null) {
                throw new ArgumentNullException(nameof(writeStream));
            }

            var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            var socketService = new SocketService();
            socketService.Connection.RemoteAddress = endPoint;
            socketService.Connection.LocalAddress = endPoint;

            if (Write(socketService, writeStream, SendOption.Connect)) {
                if (!_autoResetEvent.WaitOne(_networkConfig.AckInterval)) {
                    HandleConnect?.Invoke(socketService, null);
                }
            } else {
                HandleConnect?.Invoke(socketService, null);
            }
        }

        private string BuildID(PacketHead packetHead) {
            return packetHead.PacketId + "+" + packetHead.FragmentId;
        }
        
        private bool Acknowledge(
            SocketService socketService,
            SendOption sendOption,
            PacketHead packetHead,
            IWriteStream writeStream = null) {

            var fragment = writeStream?.ToByteFragment();
            packetHead.Length = fragment.HasValue ? (ushort)(PacketHead.GetSize() + fragment?.Count) : (ushort)PacketHead.GetSize();
            packetHead.SendOption = sendOption;

            using (var ws = PoolAllocator<IWriteStream>.GetObject()) {
                if (writeStream != null) {
                    ws.ShiftRight(writeStream.ToByteFragment());
                }
                Packet.ToBytes(packetHead, ws);
                var bf = ws.ToByteFragment();
                var result = _writer.Client.SendTo(bf.Buffer, bf.Offset, bf.Count, 
                    SocketFlags.None, socketService.Connection.RemoteAddress);
                socketService.OnWrite(result == bf.Count);
                return result == bf.Count;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public bool Write(SocketService socketService, IWriteStream writeStream) {
            if (socketService == null) {
                throw new ArgumentNullException(nameof(socketService));
            }

            if (writeStream == null) {
                throw new ArgumentNullException(nameof(writeStream));
            }

            var fragment = writeStream.ToByteFragment();
            var dataLengnth = MTU - PacketHead.GetSize();
            var messageId = socketService.SendCounter;
            // 分包
            var splits = Math.Ceiling(fragment.Count * 1.0d/ dataLengnth);
            for (var i = 0; i < splits; i++) {
                var packetHead = new PacketHead {
                    Length = (ushort)(Math.Min(fragment.Count - i*dataLengnth,dataLengnth) + PacketHead.GetSize()),
                    PacketId = messageId,
                    SendOption = SendOption.Fragment,
                    TotalBytes = (ushort) Math.Min(fragment.Count - i*dataLengnth,dataLengnth),
                    FragmentId = (ushort)i,
                    TotalFragments = (ushort)splits
                };
                Console.WriteLine(i);
                var ws = PoolAllocator<IWriteStream>.GetObject();
                ws.ShiftRight(fragment.Buffer,fragment.Offset + i * dataLengnth,packetHead.TotalBytes);
                
                Packet.ToBytes(packetHead, ws);
                var bf = ws.ToByteFragment();
                _writer.Client.SendTo(bf.Buffer,bf.Offset, bf.Count,SocketFlags.None, 
                    socketService.Connection.RemoteAddress);

                void Action() {
                    _writer.Client.SendTo(bf.Buffer,bf.Offset, bf.Count,SocketFlags.None, 
                        socketService.Connection.RemoteAddress);
                    Logger.Info("重发数据包！");
                }

                void RecycleAction() {
                    ws.Dispose();
                }

                var id = BuildID(packetHead);
                if (!_fragmentsTimer.Add(id, Action, RecycleAction)) {
                    Logger.Error("添加重发消息失败！");
                }

                socketService.PacketIds.Add(id);
            }

            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        private bool Write(SocketService socketService, ulong messageId, SendOption sendOption) {
            using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                var packetHead = new PacketHead {
                    Length = (ushort)PacketHead.GetSize(),
                    PacketId = messageId,
                    SendOption = sendOption,
                    TotalBytes = 0,
                    FragmentId = 0,
                    TotalFragments = 1
                };

                Packet.ToBytes(packetHead, writeStream);

                var byteFragment = writeStream.ToByteFragment();
                var result = _writer.Client.SendTo(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count, SocketFlags.None, 
                    socketService.Connection.RemoteAddress);

                socketService.OnWrite(result == byteFragment.Count);
                return result == byteFragment.Count;
            }
        }

        private bool Write(SocketService socketService, IWriteStream writeStream, SendOption sendOption) {
            var fragment = writeStream?.ToByteFragment();
            var packetHead = new PacketHead {
                Length = fragment.HasValue?(ushort)(PacketHead.GetSize()+fragment?.Count):(ushort)PacketHead.GetSize(),
                PacketId = socketService.SendCounter,
                SendOption = sendOption,
                TotalBytes = fragment.HasValue?(ushort)fragment?.Count:(ushort)0
            };

            var ws = PoolAllocator<IWriteStream>.GetObject();
            if (fragment.HasValue) {
                ws.ShiftRight((ByteFragment)fragment);   
            }
            Packet.ToBytes(packetHead, ws);
            var byteFragment = ws.ToByteFragment();
            var result = _writer.Client.SendTo(byteFragment.Buffer,byteFragment.Offset, byteFragment.Count,SocketFlags.None, 
                socketService.Connection.RemoteAddress);

            socketService.OnWrite(result == byteFragment.Count);
            return result == byteFragment.Count;
        }
        
        private Socket CreateSocket(AddressFamily addressFamily,EndPoint endPoint) {
            var socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);

            var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
            socket.Bind(endPoint == null?ipEndPoint:endPoint);

            return socket;
        }

        private async void ReceiveAsync(object state) {
            var networkConfig = state as NetworkConfig;
            using (var readStream = PoolAllocator<IReadStream>.GetObject()) {
                while (true) {
                    UdpReceiveResult result;
                    try {
                        result = await _writer.ReceiveAsync();
                    } catch (ObjectDisposedException) {
                        //Logger.Error(e.ToString());
                        break;
                    } catch (SocketException e) {
                        Logger.Error(e.ToString());
                        continue;
                    }

                    var length = result.Buffer.ToValue<ushort>();
                    Logger.Debug("数据长度：" + length);

                    if (length == result.Buffer.Length) {
                        readStream.CopyBuffer(result.Buffer, 0, length);
                        Logger.Debug("开始读取！");
                        try {
                            OnRead(networkConfig, result.RemoteEndPoint, readStream);
                        } catch (Exception e) {
                            Logger.Error(e);
                        }
                    }
                    readStream.Reset();
                }
            }
        }

        private void OnRead(NetworkConfig networkConfig, IPEndPoint endPoint, IReadStream readStream) {

            var packetHead = Packet.Unpacking(readStream);
            Logger.Debug("SendOption:" + packetHead.SendOption);

            if ((packetHead.SendOption & SendOption.Acknowledge) > 0) {
                if ((packetHead.SendOption & SendOption.Connect) > 0) {
                    AcknowledgeConnect(endPoint, readStream, packetHead);
                    return;
                }

                if ((packetHead.SendOption & SendOption.Disconnect) > 0) {
                    AcknowledgeDisconnect(endPoint, readStream, packetHead);
                    return;
                }

                if ((packetHead.SendOption & SendOption.Fragment) > 0) {
                    AcknowledgeFragment(endPoint, readStream, packetHead);
                    return;
                }

                if ((packetHead.SendOption & SendOption.Ack) > 0) {
                    return;
                }
            }

            if ((packetHead.SendOption & SendOption.Ack) > 0) {
                ResponseAck(endPoint, readStream, packetHead);
                return;
            }

            if ((packetHead.SendOption & SendOption.Fragment) > 0) {
                ResponseFragment(endPoint, readStream, packetHead);
                return;
            }

            if ((packetHead.SendOption & SendOption.Disconnect) > 0) {
                ResponseDisconnect(endPoint, readStream, packetHead);
            }
        }

        #region Response
        private void ResponseAck(IPEndPoint endPoint, IReadStream readStream,
            PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            Logger.Info($"接收到{endPoint}心跳包！");

            Acknowledge(socketService, SendOption.Acknowledge | SendOption.Ack, packetHead);

            if (socketService.ReconnectCount > 0) {
                Logger.Debug("关闭重连：" + socketService.ReconnectCount);
                HandleReconnect?.Invoke(socketService, false);
            }

            UdpKeepAlive.Reset(socketService);
        }

        private void ResponseFragment(IPEndPoint endPoint, IReadStream readStream,
            PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            // 应答包，可能会耗流量
            using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                writeStream.ShiftRight(readStream.RightPeek(_nextHeadOffset));
                Acknowledge(socketService, SendOption.Acknowledge | SendOption.Fragment, packetHead, writeStream);
            }

            lock (socketService) {
                // 过滤已处理的之前包
                if (socketService.RecvCounter >= packetHead.PacketId) {
                    Logger.Warn($"重复包messageId:{packetHead.PacketId} fragmentId:{packetHead.FragmentId}丢弃!");
                    return;
                }

                var byteFragment = readStream.ToByteFragment();
                if (!socketService.Packets.TryGetValue(packetHead.PacketId, out var result)) {
                    // 分包第一次处理
                    result = new Recorder() {
                        Count = packetHead.TotalFragments,
                        Flag = 0,
                        ReadStream = PoolAllocator<IReadStream>.GetObject(),
                    };
                    socketService.Packets.Add(packetHead.PacketId, result);
                }

                var readBuffer = result.ReadStream.GetReadBuffer();
                Logger.Debug(packetHead.PacketId + " " + packetHead.FragmentId);

                // 过滤已经处理的分包
                if (packetHead.TotalFragments != 1 && (result.Flag & (((ulong)1) << packetHead.FragmentId)) > 0) {
                    Logger.Warn("过滤已经处理的分包!");
                    return;
                }

                Array.Copy(byteFragment.Buffer, byteFragment.Offset, readBuffer.Buffer,
                    readBuffer.Offset + packetHead.FragmentId * packetHead.TotalBytes,
                    byteFragment.Count);

                result.Flag |= ((ulong)1) << packetHead.FragmentId;

                socketService.Packets[packetHead.PacketId] = result;
                Logger.Debug("------------------------------------");
                while (socketService.Packets.Count != 0) {
                    var next = socketService.RecvCounter + 1;
                    if (!socketService.Packets.TryGetValue(next, out var first)) {
                        Logger.Warn($"跳包!期望ID：{next}   实际ID：{packetHead.PacketId}");
                        break;
                    }

                    if (!first.IsComplete()) {
                        break;
                    }

                    socketService.RecvIncrement();
                    socketService.Packets.Remove(next);
                    socketService.OnRead(first.ReadStream);
                    first.ReadStream.Dispose();
                }
            }
        }

        private void ResponseDisconnect(IPEndPoint endPoint, IReadStream readStream,
            PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Warn("断开没有记录的连接？");
                return;
            }

            Logger.Info($"{endPoint}主动断开！");
            Acknowledge(socketService, SendOption.Acknowledge | SendOption.Disconnect, packetHead);

            OnDisconnect(socketService);
        }
        #endregion

        #region Acknowledge
        private void AcknowledgeConnect(IPEndPoint endPoint, IReadStream readStream,
            PacketHead packetHead) {
            if (_connections.ContainsKey(endPoint)) {
                Logger.Warn($"{endPoint}已经连接，无需重复连接！");
                return;
            }

            var service = new SocketService();
            service.Connection.RemoteAddress = endPoint;
            service.Connection.LocalAddress = _writer.Client.LocalEndPoint as IPEndPoint;

            if (_connections.TryAdd(endPoint, service)) {
                _autoResetEvent.Set();
                UdpKeepAlive.AddConnection(service,
                    () => HandleReconnect?.Invoke(service, true),
                    () => {
                        HandleReconnect?.Invoke(service, false);
                        OnDisconnect(service);
                    });

                HandleConnect?.Invoke(service, readStream);

                Logger.Info($"{endPoint}连接成功！");
            } else {
                Logger.Error($"添加连接{endPoint}失败!");
            }
        }

        private void AcknowledgeDisconnect(IPEndPoint endPoint, IReadStream readStream,
            PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            OnDisconnect(socketService);

            Logger.Info($"应答{endPoint}主动断开连接！");
        }

        private void AcknowledgeFragment(IPEndPoint endPoint, IReadStream readStream,
            PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }
            var id = BuildID(packetHead);
            if (!(_fragmentsTimer.Remove(id) && socketService.PacketIds.Remove(id))) {
                return;
            }

            socketService.OnAcknowledge(true, readStream);
            Logger.Info($"{endPoint}：{packetHead.PacketId}消息发送完成！");
        }
        #endregion

        public void Disconnect(SocketService socketService) {
            if (socketService == null) {
                throw new ArgumentNullException($"{nameof(socketService)} 为 null ！");
            }

            Write(socketService, socketService.SendCounter, SendOption.Disconnect);
        }

        private void OnDisconnect(SocketService socketService) {
            if (_connections.TryRemove(socketService.Connection.RemoteAddress,
                out socketService)) {

                UdpKeepAlive.Remove(socketService);
                foreach (var id in socketService.PacketIds) {
                    _fragmentsTimer.Remove(id);    
                }
                socketService.OnDisconnect();
                Logger.Info($"断开{socketService.Connection.RemoteAddress}连接！");
            } else {
                Logger.Warn("是否已经断开?");
            }
        }
        
        
        public void Dispose() {
            Logger.Info("Dispose");

            foreach (var item in _connections.Values) {
                Disconnect(item);
            }

            _tokenSource.Cancel();
            _writer.Close();
            _connections.Clear();

            UdpKeepAlive.Stop();
            _fragmentsTimer.Stop();
        }
    }
}