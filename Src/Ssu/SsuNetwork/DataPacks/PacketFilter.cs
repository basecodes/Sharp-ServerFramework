using System;
using System.Collections.Concurrent;
using Ssc.SscLog;

namespace Ssu.SsuNetwork.DataPacks {
    internal sealed class PacketFilter {
        private static readonly Logger Logger = LogManager.GetLogger<PacketFilter>(LogType.Low);
        private readonly ConcurrentQueue<Guid> _filter;
        private readonly int _count;

        public PacketFilter(int count) {
            if (count < 0) throw new ArgumentException(nameof(count));

            _count = count;
            _filter = new ConcurrentQueue<Guid>();
        }

        public void AddPacketID(Guid guid) {
            if (_filter.Count >= _count) {
                if (_filter.TryDequeue(out var result))
                    _filter.Enqueue(guid);
                else
                    Logger.Warn($"消息Guid:{guid}进队列失败！");
            } else {
                _filter.Enqueue(guid);
            }
        }

        public bool Check(Guid guid) {
            foreach (var item in _filter)
                if (item == guid)
                    return true;

            return false;
        }

        public void Clear() {
            while (_filter.Count > 0) {
                _filter.TryDequeue(out _);
            }
        }
    }
}