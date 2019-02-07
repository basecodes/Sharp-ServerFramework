using System;
using System.Threading;

namespace Ssc.SscAlgorithm.SscQueue {
    internal class TimerQueueEvent : ICloneable {
        public WaitCallback CallBack { get; set; }
        public object State { get; set; }
        public TimeSpan TimeOut { get; set; }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}