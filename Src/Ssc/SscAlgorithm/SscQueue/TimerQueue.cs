using System;
using System.Threading;

namespace Ssc.SscAlgorithm.SscQueue {
    public class TimerQueue {
        private readonly object synchNewEventLock = new object();

        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private readonly Heap<double, TimerQueueEvent> eventQueue;

        private readonly Thread eventThread;

        private bool initialized;

        private TimerQueueEvent newEvent;

        public TimerQueue(int queueSize) {

            int Compare(double value1, double value2) {
                return (int)(value1 - value2);
            }

            eventQueue = new Heap<double, TimerQueueEvent>(queueSize,Compare, null);
            eventThread = new Thread(EventProcessor);
        }

        private double NowInMilliSeconds => TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds;

        public void Initialize() {
            initialized = true;
            eventThread.Start();
        }

        public void SetTimer(WaitCallback callBack, object state, double waitTimeInMs) {
            if (!initialized)
                throw new InvalidOperationException(
                    "method Initialize must be called before using the TimerQueue for scheduling actions");

            lock (synchNewEventLock) {
                newEvent = new TimerQueueEvent {
                    CallBack = callBack,
                    State = state,
                    TimeOut = new TimeSpan(DateTime.Now.AddMilliseconds(waitTimeInMs).Ticks)
                };

                eventQueue.Push(newEvent.TimeOut.TotalMilliseconds, newEvent);

                autoResetEvent.Set();
            }

            ;
        }

        private void EventProcessor() {
            autoResetEvent.WaitOne();
            while (true) {
                TimerQueueEvent minEvent = null;

                lock (synchNewEventLock) {
                    minEvent = eventQueue.Peek();
                }

                if (minEvent == null) {
                    autoResetEvent.WaitOne();
                } else {
                    var waitTimeInMs = minEvent.TimeOut.TotalMilliseconds - NowInMilliSeconds;
                    if (!autoResetEvent.WaitOne((int) waitTimeInMs)) {
                        lock (synchNewEventLock) {
                            minEvent = eventQueue.Pop();
                        }

                        ThreadPool.QueueUserWorkItem(minEvent.CallBack, minEvent.State);
                    }
                }
            }
        }
    }
}