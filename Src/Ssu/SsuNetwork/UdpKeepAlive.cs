using System;
using System.Collections.Concurrent;
using System.Threading;
using Ssu.SsuNetwork.Sockets;
using Timer = System.Timers.Timer;

namespace Ssu.SsuNetwork {
    internal sealed class UdpKeepAlive {

        private static readonly Ssc.SscLog.Logger Logger = Ssc.SscLog.LogManager.GetLogger<UdpKeepAlive>(Ssc.SscLog.LogType.Low);
        private static ConcurrentDictionary<SocketService, Timer> _dict;
        private static int _dueTime = Timeout.Infinite;
        private static int _period = Timeout.Infinite;
        private static int _maxReconnectCount = 0;
        private static bool _running = false;

        static UdpKeepAlive(){
            _dict = new ConcurrentDictionary<SocketService, Timer>();
        }

        public static void Start(int period,int maxReconnectCount) {
            _dueTime = period;
            _period = period;
            _maxReconnectCount = maxReconnectCount;
            _running = true;
        }

        private static TimerCallback CreateCallback(Action reconnect,Action disconnect) {
            TimerCallback callback = (state) => {
                var socketService = state as SocketService;
                Logger.Debug(socketService.ReconnectCount);

                if (socketService.ReconnectCount == 0) {
                    reconnect?.Invoke();
                }
                if (socketService.ReconnectCount >= _maxReconnectCount) {
                    disconnect?.Invoke();
                    socketService.ReconnectCount = 0;
                } else {
                    socketService.ReconnectCount++;
                }
            };
            return callback;
        }

        public static void AddConnection(SocketService socketService, Action reconnect, Action disconnect) {
            if (!_running) {
                return;
            }

            if (socketService == null) {
                throw new ArgumentNullException(nameof(socketService));
            }

            var callback = CreateCallback(reconnect, disconnect);

            var timer = new Timer(_period);
            timer.AutoReset = true;
            timer.Elapsed += (sender, e) =>  callback.Invoke(socketService);

            timer.Start();
            if (!_dict.TryAdd(socketService, timer)) {
                Logger.Error($"{socketService}心跳包添加失败！");
                return;
            }

            Logger.Info("打开定时器！");
        }

        public static void Stop() {
            if (!_running) {
                return;
            }

            foreach (var item in _dict.Keys) {
                Stop(item);
            }

            _dueTime = Timeout.Infinite;
             _period = Timeout.Infinite;
            _maxReconnectCount = 0;
            _running = false;
    }

        public static void Stop(SocketService socketService) {
            if (!_running) {
                return;
            }
            if (socketService == null) {
                throw new ArgumentNullException(nameof(socketService));
            }

            if (!_dict.TryGetValue(socketService, out var timer)) {
                Logger.Error($"获取{socketService}心跳包失败！");
                return;
            }

            timer.Stop();
            timer.Close();
            socketService.ReconnectCount = 0;
        }

        public static void Remove(SocketService socketService) {
            if (!_running) {
                return;
            }
            if (socketService == null) {
                throw new ArgumentNullException(nameof(socketService));
            }

            if (!_dict.TryRemove(socketService, out var timer)) {
                Logger.Error($"删除{socketService}心跳包失败！");
                return;
            }

            timer.Stop();
            timer.Close();
            socketService.ReconnectCount = 0;
            Logger.Info("关闭定时器！");
        }

        public static void Reset(SocketService socketService) {
            if (!_running) {
                return;
            }

            if (socketService == null) {
                throw new ArgumentNullException(nameof(socketService));
            }

            if (!_dict.TryGetValue(socketService, out var timer)) {
                Logger.Error($"获取{socketService}心跳包失败！");
                return;
            }

            timer.Stop();
            timer.Start();
            socketService.ReconnectCount = 0;
            Logger.Info("重置定时器！");
        }
    }
}
