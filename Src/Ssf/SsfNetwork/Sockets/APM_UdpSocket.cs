using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Ssc.SscConfiguration;
using Ssc.SscExtension;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssc.SscTemplate;
using Ssf.SsfNetwork.Packets;

namespace Ssf.SsfNetwork.Sockets {
    public sealed class APM_UdpSocket : SocketCore {

        public override SocketStatistics SocketStatistics { get; }
        public override event Func<SocketService, IReadStream,IWriteStream,bool> HandleAccept;
        public override event Action<SocketService, IReadStream> HandleConnect;

        private static readonly Logger Logger = LogManager.GetLogger<APM_UdpSocket>(LogType.Low);
        private ConcurrentDictionary<EndPoint, SocketService> _connections;
        private UdpKeepAlive _udpKeepAlive;
        private FragmentsTimer _fragmentTimer;
        private const int MTU = 508;

        private Socket _readSocket;
        private object _senderLock = new object();
        private object _receiveLock = new object();

        private int _nextHeadOffset;
        public APM_UdpSocket(int nextHeadOffset) {
            SocketStatistics = new SocketStatistics();

            _nextHeadOffset = nextHeadOffset;

            PoolAllocator<SocketService>.SetPool(args =>
            ObjectFactory.GetActivator<SocketService>(typeof(SocketService).GetConstructors().First())());

            PoolAllocator<IWriteStream>.SetPool(args =>
                ObjectFactory.GetActivator<IWriteStream>(typeof(WriteStream).GetConstructors().First())());

            PoolAllocator<IReadStream>.SetPool(args =>
                ObjectFactory.GetActivator<IReadStream>(typeof(ReadStream).GetConstructors().First())());
        }

        public override void ConnectionGenerator(IPEndPoint remoteAddress, IWriteStream writeStream, Action<SocketService> action) {
            if (remoteAddress == null) {
                throw new ArgumentNullException(nameof(remoteAddress));
            }

            if (_connections.TryGetValue(remoteAddress, out var socketService)) {
                Logger.Warn($"已经存在连接{remoteAddress}!无需重复连接");
                WriteMessage(socketService, SendOption.Acknowledge | SendOption.Connect, null);
                return;
            }

            socketService = PoolAllocator<SocketService>.GetObject();
            socketService.Connection.RemoteAddress = remoteAddress;

            if (_connections.TryAdd(remoteAddress, socketService)) {

                action?.Invoke(socketService);
                Acknowledge(socketService, SendOption.Acknowledge | SendOption.Connect,new PacketHead(), writeStream);
                CreateAck(socketService);

                Logger.Info($"创建{remoteAddress}连接心跳包！");
            } else {
                Logger.Error($"添加连接{remoteAddress}失败!");
            }
        }

        private Socket CreateSocket(AddressFamily addressFamily, EndPoint endPoint) {
            var socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);

            if (endPoint == null) {
                return socket;
            }

            try {
                Logger.Debug("Bind:" + endPoint);
                socket.Bind(endPoint);
            } catch (Exception e) {
                Logger.Error(e);
            }

            return socket;
        }

        private void CreateAck(SocketService socketService) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return;
            }

            _udpKeepAlive?.AddAck(socketService, () => {
                WriteMessage(socketService, SendOption.Ack, 0, null);
                SocketStatistics.LogAckSend();
                Logger.Debug($"发给{socketService.Connection.RemoteAddress}心跳包！");
            }, () => {
                OnDisconnect(socketService);
            });
        }

        #region Accept

        public override void Start(SocketConfig socketConfig) {
            if (socketConfig == null) {
                throw new ArgumentNullException(nameof(socketConfig));
            }

            _connections = new ConcurrentDictionary<EndPoint, SocketService>();

            if (socketConfig.OpenKeepAlive) {
                _udpKeepAlive = new UdpKeepAlive();
                _udpKeepAlive.Start(socketConfig.KeepAliveInterval, socketConfig.ReconnectMaxCount);

            }

            if (socketConfig.OpenFragmentResend) {
                _fragmentTimer = new FragmentsTimer();
                _fragmentTimer.Start(socketConfig.FragmentInterval);
            }

            var endPoint = new IPEndPoint(IPAddress.Parse(socketConfig.IP), socketConfig.Port);
            _readSocket = CreateSocket(endPoint.AddressFamily, endPoint);

            var socketService = PoolAllocator<SocketService>.GetObject();
            socketService.Connection.LocalAddress = endPoint;

            BeginRead(socketService);
        }

        #endregion Accept

        #region Connect

        public override void Connect(SocketConfig socketConfig) {
            using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                Connect(socketConfig, writeStream);
            }
        }

        public override void Connect(SocketConfig socketConfig, IWriteStream writeStream) {
            if (socketConfig == null) {
                throw new ArgumentNullException(nameof(socketConfig));
            }

            if (writeStream == null) {
                throw new ArgumentNullException(nameof(writeStream));
            }

            _connections = new ConcurrentDictionary<EndPoint, SocketService>();

            if (socketConfig.OpenKeepAlive) {
                _udpKeepAlive = new UdpKeepAlive();
                _udpKeepAlive.Start(socketConfig.KeepAliveInterval, socketConfig.ReconnectMaxCount);
            }

            if (socketConfig.OpenFragmentResend) {
                _fragmentTimer = new FragmentsTimer();
                _fragmentTimer.Start(socketConfig.FragmentInterval);
            }

            var endPoint = new IPEndPoint(IPAddress.Parse(socketConfig.IP), socketConfig.Port);
            var address = new IPEndPoint(IPAddress.Parse(socketConfig.IP), 0);

            _readSocket = CreateSocket(endPoint.AddressFamily, address);

            var socketService = PoolAllocator<SocketService>.GetObject();
            socketService.Connection.RemoteAddress = endPoint;
            socketService.Connection.LocalAddress = _readSocket.LocalEndPoint;

            BeginRead(socketService);

            WriteMessage(socketService, SendOption.Connect, socketService.SendCounter, writeStream);
        }

        #endregion Connect

        #region Write

        public override bool Write(SocketService socketService, IWriteStream writeSteam) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return false;
            }

            if (writeSteam == null) {
                socketService.OnError($"{nameof(writeSteam)} 为 null ！");
                return false;
            }

            return WriteMessage(socketService, SendOption.Fragment, writeSteam);
        }

        private bool StartWrite(SocketService socketService, byte[] buffer, int offset, int count, IWriteStream writeStream,
            bool isRecycle = false) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return false;
            }

            if (socketService.Connection.RemoteAddress == null) {
                Logger.Error($"远程地址不能为null!");
                return false;
            }

            lock (_senderLock) {
                try {
                    _readSocket.BeginSendTo(buffer, offset, count, SocketFlags.None,
                        socketService.Connection.RemoteAddress, result => {
                            var bytesSended = 0;
                            try {
                                lock (_senderLock) {
                                    bytesSended = _readSocket.EndSendTo(result);
                                }
                            } catch (SocketException socketException) {
                                socketService.OnError(socketException.ToString());
                                OnDisconnect(socketService);
                            } catch (ObjectDisposedException objectDisposedException) {
                                socketService.OnError(objectDisposedException.ToString());
                                OnDisconnect(socketService);
                            } catch (InvalidOperationException invalidOperationException) {
                                socketService.OnError(invalidOperationException.ToString());
                                OnDisconnect(socketService);
                            }

                            if (bytesSended != count) {
                                Logger.Warn("未完全发送完，自动丢弃!");
                            } else {
                                Logger.Debug($"发送{socketService.Connection.RemoteAddress}：{count}字节数据！");
                                socketService.OnWrite(true);
                            }

                            if (isRecycle) {
                                writeStream.Dispose();
                            }
                        }, null);

                } catch (SocketException socketException) {
                    Logger.Error(socketException.ToString());
                    socketService.OnError(socketException.ToString());
                    OnDisconnect(socketService);
                } catch (ObjectDisposedException objectDisposedException) {
                    socketService.OnError(objectDisposedException.ToString());
                    OnDisconnect(socketService);
                } catch (InvalidOperationException invalidOperationException) {
                    socketService.OnError(invalidOperationException.ToString());
                    OnDisconnect(socketService);
                }
            }
            return true;
        }

        private bool Acknowledge(
            SocketService socketService,
            SendOption sendOption,
            PacketHead packetHead,
            IWriteStream writeStream = null) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return false;
            }
            var fragment = writeStream?.ToByteFragment();
            packetHead.Length = fragment.HasValue ? (ushort)(PacketHead.GetSize() + fragment?.Count) : (ushort)PacketHead.GetSize();
            packetHead.SendOption = sendOption;

            var ws = PoolAllocator<IWriteStream>.GetObject();
            if (writeStream != null) {
                ws.ShiftRight(writeStream.ToByteFragment());
            }
            Packet.ToBytes(packetHead, ws);
            var byteSegment = ws.ToByteFragment();

            var result = StartWrite(socketService, byteSegment.Buffer, byteSegment.Offset, byteSegment.Count, ws, true);

            SocketStatistics.LogUnreliableSend();
            SocketStatistics.LogTotalBytesSent(packetHead.Length);
            return true;
        }

        protected override bool WriteMessage(
            SocketService socketService,
            SendOption sendOption,
            ulong messageId,
            IWriteStream writeStream) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return false;
            }

            var fragment = writeStream?.ToByteFragment();
            var packetHead = new PacketHead {
                Length = fragment.HasValue ? (ushort)(PacketHead.GetSize() + fragment?.Count) : (ushort)PacketHead.GetSize(),
                PacketId = messageId,
                SendOption = sendOption,
                TotalBytes = fragment.HasValue ? (ushort)fragment?.Count : (ushort)0,
                FragmentId = 0,
                TotalFragments = 1
            };

            var ws = PoolAllocator<IWriteStream>.GetObject();
            if (fragment.HasValue) {
                ws.ShiftRight((ByteFragment)fragment);
            }
            Packet.ToBytes(packetHead, ws);
            var byteSegment = ws.ToByteFragment();
            var result = StartWrite(socketService, byteSegment.Buffer, byteSegment.Offset, byteSegment.Count, ws, true);

            SocketStatistics.LogUnreliableSend();
            SocketStatistics.LogTotalBytesSent(packetHead.Length);
            return result;
        }

        protected override bool WriteMessage(
            SocketService socketService,
            SendOption sendOption,
            IWriteStream writeStream) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return false;
            }

            if (writeStream == null) {
                Logger.Error($"{nameof(writeStream)} 为 null ！");
                return false;
            }

            var fragment = writeStream.ToByteFragment();
            var dataLengnth = MTU - PacketHead.GetSize();
            var messageId = socketService.SendCounter;
            // 分包
            var splits = Math.Ceiling(fragment.Count * 1.0d / dataLengnth);
            for (var i = 0; i < splits; i++) {
                var packetHead = new PacketHead {
                    Length = (ushort)(Math.Min(fragment.Count - i * dataLengnth, dataLengnth) + PacketHead.GetSize()),
                    PacketId = messageId,
                    SendOption = sendOption,
                    TotalBytes = (ushort)Math.Min(fragment.Count - i * dataLengnth, dataLengnth),
                    FragmentId = (ushort)i,
                    TotalFragments = (ushort)splits
                };

                var ws = PoolAllocator<IWriteStream>.GetObject();
                ws.ShiftRight(fragment.Buffer, fragment.Offset + i * dataLengnth, packetHead.TotalBytes);
                Packet.ToBytes(packetHead, ws);

                var byteSegment = ws.ToByteFragment();

                void Action() {
                    StartWrite(socketService, byteSegment.Buffer, byteSegment.Offset, byteSegment.Count, ws);
                    SocketStatistics.LogFragmentedSend();
                    SocketStatistics.LogDataBytesSent(packetHead.TotalBytes);
                    SocketStatistics.LogTotalBytesSent(packetHead.Length);
                    Logger.Info("重发数据包！");
                }

                void RecycleAction() {
                    ws.Dispose();
                }

                var id = BuildID(packetHead);
                _fragmentTimer?.Add(id, Action, RecycleAction);

                StartWrite(socketService, byteSegment.Buffer, byteSegment.Offset, byteSegment.Count, writeStream);

                socketService.PacketIds.Add(id);
                SocketStatistics.LogFragmentedSend();
                SocketStatistics.LogReliableSend();
                SocketStatistics.LogDataBytesSent(packetHead.TotalBytes);
                SocketStatistics.LogTotalBytesSent(packetHead.Length);
            }

            return true;
        }

        #endregion Write

        private string BuildID(PacketHead packetHead) {
            return packetHead.PacketId + "+" + packetHead.FragmentId;
        }

        #region Read

        private void BeginRead(SocketService socketService) {
            if (socketService == null) {
                Logger.Error($"{nameof(socketService)} 为 null ！");
                return;
            }

            var endPoint = socketService.Connection.LocalAddress;
            var readStream = PoolAllocator<IReadStream>.GetObject();
            var byteFragment = readStream.GetReadBuffer();
            try {
                lock (_receiveLock) {
                    _readSocket.BeginReceiveFrom(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count, SocketFlags.None,
                        ref endPoint, result => {
                            EndPoint remoteEndPoint = new IPEndPoint(
                                endPoint.AddressFamily == AddressFamily.InterNetwork
                                    ? IPAddress.Any
                                    : IPAddress.IPv6Any,
                                0);

                            var bytesReceived = 0;
                            try {
                                lock (_receiveLock) {
                                    bytesReceived = _readSocket.EndReceiveFrom(result, ref remoteEndPoint);
                                }
                            } catch (SocketException e) {
                                socketService?.OnError(e.ToString());
                                readStream.Dispose();
                                BeginRead(socketService);
                                return;
                            } catch (ObjectDisposedException objectDisposedException) {
                                socketService?.OnError(objectDisposedException.ToString());
                                readStream.Dispose();
                                return;
                            } catch (InvalidOperationException invalidOperationException) {
                                socketService?.OnError(invalidOperationException.ToString());
                                readStream.Dispose();
                                return;
                            }
                            if (bytesReceived == 0) {
                                return;
                            }

                            BeginRead(socketService);

                            var length = byteFragment.Buffer.ToValue<ushort>(byteFragment.Offset);
                            if (length == bytesReceived) {
                                readStream.SetReadCount(length);
                                try {
                                    OnRead(remoteEndPoint, readStream);
                                } catch (Exception e) {
                                    Logger.Error(e);
                                }
                            } else {
                                Logger.Warn($"包数据未接收全,总长{length}，接收长度{bytesReceived}，自动丢弃！");
                            }

                            readStream.Dispose();
                        }, null);
                }
            } catch (SocketException e) {
                socketService?.OnError(e.ToString());
                BeginRead(socketService);
            } catch (ObjectDisposedException objectDisposedException) {
                socketService?.OnError(objectDisposedException.ToString());
            } catch (InvalidOperationException invalidOperationException) {
                socketService?.OnError(invalidOperationException.ToString());
            }
        }

        private void OnRead(EndPoint endPoint, IReadStream readStream) {
            var packetHead = Packet.Unpacking(readStream);

            if ((packetHead.SendOption & SendOption.Acknowledge) > 0) {
                if ((packetHead.SendOption & SendOption.Connect) > 0) {
                    AcknowledgeConnect(endPoint, readStream,packetHead);
                    return;
                }

                if ((packetHead.SendOption & SendOption.Disconnect) > 0) {
                    AcknowledgeDisonnect(endPoint, readStream,packetHead);
                    return;
                }

                if ((packetHead.SendOption & SendOption.Fragment) > 0) {
                    AcknowledgeFragment(endPoint, readStream,packetHead);
                    return;
                }

                if ((packetHead.SendOption & SendOption.Ack) > 0) {
                    AcknowledgeAck(endPoint, readStream,packetHead);
                    return;
                }
            }

            if ((packetHead.SendOption & SendOption.Ack) > 0) {
                ResponseAck(endPoint, readStream,packetHead);
                return;
            }

            if ((packetHead.SendOption & SendOption.Fragment) > 0) {
                ResponseFragment(endPoint, readStream, packetHead);
                return;
            }

            if ((packetHead.SendOption & SendOption.Connect) > 0) {
                ResponseConnect(endPoint, readStream, packetHead);
                return;
            }

            if ((packetHead.SendOption & SendOption.Disconnect) > 0) {
                ResponseDisconnect(endPoint, readStream, packetHead);
            }
        }

        #region Response
        private void ResponseAck(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }
            Logger.Info($"接收到{endPoint}心跳包！");

            Acknowledge(socketService, SendOption.Acknowledge | SendOption.Ack, packetHead);
            SocketStatistics.LogAcknowledgementSend();
            SocketStatistics.LogUnreliableReceive();
            SocketStatistics.LogAcknowledgementSend();
            SocketStatistics.LogAckReceive();
            SocketStatistics.LogTotalBytesReceived(PacketHead.GetSize());
        }

        private void ResponseFragment(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            using(var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                writeStream.ShiftRight(readStream.RightPeek(_nextHeadOffset));
                Acknowledge(socketService, SendOption.Acknowledge | SendOption.Fragment, packetHead,writeStream);
            }

            SocketStatistics.LogAcknowledgementSend();
            SocketStatistics.LogFragmentedReceive();

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
                Logger.Debug($"MessageID:{packetHead.PacketId} 段数：{packetHead.FragmentId} 总段数：{packetHead.TotalFragments} 当前全部包数：" + socketService.Packets.Count);

                while (socketService.Packets.Count != 0) {
                    var next = socketService.RecvCounter + 1;
                    if(!socketService.Packets.TryGetValue(next,out var first)) {
                        break;
                    }

                    if (!first.IsComplete()) {
                        break;
                    }

                    Logger.Debug("MessageID:" + next + " 包数：" + socketService.Packets.Count + " 完成！");
                    socketService.RecvIncrement();
                    socketService.Packets.Remove(next);
                    socketService.OnRead(first.ReadStream);
                    first.ReadStream.Dispose();
                }
            }

            SocketStatistics.LogDataBytesReceived(packetHead.TotalBytes);
            SocketStatistics.LogTotalBytesReceived(packetHead.Length);
            SocketStatistics.LogReliableReceive();
        }

        private void ResponseConnect(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Warn($"已经存在连接{endPoint}!无需重复连接");
                Acknowledge(socketService, SendOption.Acknowledge | SendOption.Connect, packetHead);
                return;
            }

            var service = PoolAllocator<SocketService>.GetObject();
            service.Connection.RemoteAddress = endPoint;

            if (_connections.TryAdd(endPoint, service)) {
                using (var writeStream = PoolAllocator<IWriteStream>.GetObject()) {
                    var disconnect = HandleAccept?.Invoke(service, readStream, writeStream);
                    var byteSegment = writeStream.ToByteFragment();
                    Acknowledge(service, SendOption.Acknowledge | SendOption.Connect, packetHead, writeStream);
                    if (disconnect.GetValueOrDefault(true)) {
                        CreateAck(service);
                    } else {
                        Disconnect(service);
                    }
                    SocketStatistics.LogAcknowledgementSend();
                    SocketStatistics.LogUnreliableReceive();
                    SocketStatistics.LogDataBytesReceived(byteSegment.Count);
                    SocketStatistics.LogTotalBytesReceived(writeStream.ToByteFragment().Count);
                }

                Logger.Info($"创建{endPoint}连接心跳包！");
            } else {
                Logger.Error($"添加连接{endPoint}失败!");
            }
        }

        private void ResponseDisconnect(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Warn("断开没有记录的连接？");
                return;
            }

            Logger.Info($"{endPoint}主动断开！");
            Acknowledge(socketService, SendOption.Acknowledge | SendOption.Disconnect, packetHead);

            SocketStatistics.LogAcknowledgementSend();
            SocketStatistics.LogUnreliableReceive();
            SocketStatistics.LogTotalBytesReceived(PacketHead.GetSize());

            OnDisconnect(socketService);
        }
        #endregion

        #region Acknowledge
        private void AcknowledgeConnect(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (_connections.ContainsKey(endPoint)) {
                Logger.Warn($"{endPoint}已经连接，无需重复连接！");
                return;
            }

            var service = PoolAllocator<SocketService>.GetObject();
            service.Connection.RemoteAddress = endPoint;
            service.Connection.LocalAddress = _readSocket.LocalEndPoint as IPEndPoint;

            if (_connections.TryAdd(endPoint, service)) {

                CreateAck(service);
                HandleConnect?.Invoke(service, readStream);

                SocketStatistics.LogUnreliableReceive();
                SocketStatistics.LogAcknowledgementReceive();
                SocketStatistics.LogDataBytesReceived(packetHead.TotalBytes);
                SocketStatistics.LogTotalBytesReceived(packetHead.Length);
                Logger.Info($"{endPoint}连接成功！");
            } else {
                Logger.Error($"添加连接{endPoint}失败!");
            }
        }

        private void AcknowledgeDisonnect(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            OnDisconnect(socketService);

            SocketStatistics.LogUnreliableReceive();
            SocketStatistics.LogAcknowledgementReceive();
            SocketStatistics.LogDataBytesReceived(packetHead.TotalBytes);
            SocketStatistics.LogTotalBytesReceived(packetHead.Length);
            Logger.Info($"应答{endPoint}主动断开连接！");
        }

        private void AcknowledgeFragment(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            var id = BuildID(packetHead);
            _fragmentTimer?.Remove(id);
            if (!socketService.PacketIds.Remove(id)) {
                return;
            }

            socketService.OnAcknowledge(true, readStream);

            SocketStatistics.LogUnreliableReceive();
            SocketStatistics.LogAcknowledgementReceive();
            SocketStatistics.LogDataBytesReceived(packetHead.TotalBytes);
            SocketStatistics.LogTotalBytesReceived(packetHead.Length);
            Logger.Info($"{endPoint}：{packetHead.PacketId}消息发送完成！");
        }

        private void AcknowledgeAck(EndPoint endPoint, IReadStream readStream,PacketHead packetHead) {
            if (!_connections.TryGetValue(endPoint, out var socketService)) {
                Logger.Error($"{endPoint}连接没有添加！");
                return;
            }

            Logger.Info($"应答{endPoint}心跳包！");
            _udpKeepAlive?.RemoveDisconnectAction(socketService);

            SocketStatistics.LogUnreliableReceive();
            SocketStatistics.LogAcknowledgementReceive();
            SocketStatistics.LogAckReceive();
            SocketStatistics.LogDataBytesReceived(packetHead.TotalBytes);
            SocketStatistics.LogTotalBytesReceived(packetHead.Length);
        }
        #endregion


        #endregion Read

        #region Disconnect

        public override void Disconnect(SocketService socketService) {
            if (socketService == null) {
                throw new ArgumentNullException($"{nameof(socketService)} 为 null ！");
            }

            WriteMessage(socketService, SendOption.Disconnect, socketService.SendCounter, null);
        }

        protected override void OnDisconnect(SocketService socketService) {
            if (_connections.TryRemove(socketService.Connection.RemoteAddress,out _)) {
                Logger.Info($"断开{socketService.Connection.RemoteAddress}连接！");
                _udpKeepAlive?.Remove(socketService);

                foreach (var id in socketService.PacketIds) {
                    _fragmentTimer?.Remove(id);
                }
                socketService.OnDisconnect();
                socketService.Recycle();
            } else {
                Logger.Warn("是否已经断开?");
            }
        }

        public override void Dispose(bool disposing) {
            if (disposing) {
                Logger.Debug("Dispose");

                foreach (var item in _connections.Values) {
                    Disconnect(item);
                }

                _udpKeepAlive?.Stop();
                _fragmentTimer?.Stop();

                _readSocket?.Close();
            }

            base.Dispose(disposing);
        }

        #endregion Disconnect
    }
}