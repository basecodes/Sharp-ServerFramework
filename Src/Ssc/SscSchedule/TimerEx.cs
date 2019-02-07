using System;
using System.Threading;
using Ssc.Ssc;

namespace Ssc.SscSchedule {
    public class TimerEx : IRecyclable {
        
        private readonly Timer _timer;

        public Action callback { get; set; }
        public TimerEx() {
            _timer = new Timer(obj =>  callback?.Invoke(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Recycle() {
            Change(Timeout.Infinite, Timeout.Infinite);
            callback = null;
        }

        public bool Start(int dueTime, int period) {
            return Change(dueTime, period);
        }

        public bool Start(int dueTime) {
            return Start(dueTime, Timeout.Infinite);
        }

        private bool Change(int dueTime, int period) {
            return _timer.Change(dueTime, period);
        }

        public bool Reset(int dueTime) {
            Stop();
            return Start(dueTime, Timeout.Infinite);
        }

        public bool Reset(int dueTime, int period) {
            Stop();
            return Start(dueTime, period);
        }

        public bool Stop() {
            return Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}