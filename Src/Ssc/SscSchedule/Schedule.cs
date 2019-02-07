using System;
using System.Threading;

namespace Ssc.SscSchedule {
    public class ElapsedEventArgs : EventArgs {
        public int TotalTime { get; internal set; }
    }

    public class Schedule {
        #region Public events

        public event EventHandler<ElapsedEventArgs> Elapsed;

        #endregion Public events

        #region Private methods

        private void TimerCallBack(object state) {
            lock (_taskTimer) {
                if (!_running || _performingTasks) return;

                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _performingTasks = true;
            }

            var elapsedEventArgs = state as ElapsedEventArgs;
            try {
                Elapsed?.Invoke(this, elapsedEventArgs);
            } catch (Exception e) {
                Console.WriteLine(e);
            } finally {
                lock (_taskTimer) {
                    _performingTasks = false;
                    if (_running) _taskTimer.Change(Period, Timeout.Infinite);

                    Monitor.Pulse(_taskTimer);
                }
            }
        }

        #endregion Private methods

        #region Public fields

        public int Period { get; set; }

        public bool RunOnStart { get; set; }

        #endregion Public fields

        #region Private fields

        private readonly Timer _taskTimer;

        private volatile bool _running;

        private volatile bool _performingTasks;

        private readonly ElapsedEventArgs _elapsedEventArgs;

        #endregion Private fields

        #region Constructors

        public Schedule(int period)
            : this(period, false) {
        }

        public Schedule(int period, bool runOnStart) {
            Period = period;
            RunOnStart = runOnStart;
            _elapsedEventArgs = new ElapsedEventArgs();
            _taskTimer = new Timer(TimerCallBack, _elapsedEventArgs, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion Constructors

        #region Public methods

        public void Start() {
            _running = true;
            _taskTimer.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
        }

        public void Stop() {
            lock (_taskTimer) {
                _running = false;
                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public void WaitToStop() {
            lock (_taskTimer) {
                while (_performingTasks) Monitor.Wait(_taskTimer);
            }
        }

        #endregion Public methods
    }
}