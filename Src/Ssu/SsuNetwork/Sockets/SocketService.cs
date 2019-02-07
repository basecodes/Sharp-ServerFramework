using System;
using System.Collections.Generic;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssu.SsuNetwork.Sockets {
    public class SocketService {
        private static readonly Logger Logger = LogManager.GetLogger<SocketService>(LogType.Low);

        public event Action HandleDisconnect;
        public event Action<bool> HandleWrite;
        public event Action<IReadStream> HandleRead;
        public event Action<string> HandleError;
        public event Action<bool,IReadStream> HandleAcknowledge;

        public int ReconnectCount { get; set; }

        private ulong _recvCounter;
        public ulong RecvCounter {
            get {
                lock (this) {
                    return _recvCounter;
                }
            }
        }

        private ulong _sendCounter;
        public ulong SendCounter {
            get {
                lock (this) {
                    return ++_sendCounter;
                }
            }
        }
        
        public Connection Connection { get; }
        public HashSet<string> PacketIds { get; }
        public Dictionary<ulong, Recorder> Packets { get; }
        
        public SocketService() {
            Connection = new Connection();
            PacketIds = new HashSet<string>();
            Packets = new Dictionary<ulong, Recorder>();
        }
        
        public void RecvIncrement() {
            _recvCounter++;
        }

        public void OnDisconnect() {
            if (HandleDisconnect == null)
                Logger.Debug($"{nameof(HandleDisconnect)}没添加事件！");
            else
                HandleDisconnect.Invoke();
        }

        public void OnWrite(bool success) {
            HandleWrite?.Invoke(success);
        }

        public void OnRead(IReadStream readStream) {
            if (HandleRead == null)
                Logger.Debug($"{nameof(HandleRead)}没添加事件！");
            else
                try {
                    HandleRead.Invoke(readStream);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
        }

        public void OnAcknowledge(bool success,IReadStream readStream) {
            if (HandleAcknowledge == null)
                Logger.Debug($"{nameof(HandleAcknowledge)}没添加事件！");
            else
                HandleAcknowledge.Invoke(success,readStream);
        }

        public void OnError(string error) {
            if (HandleError == null)
                Logger.Debug($"{nameof(HandleError)}没添加事件！");
            else
                HandleError.Invoke(error);
        }
    }
}