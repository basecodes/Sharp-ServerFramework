using System;

namespace Ssu.Ssu {

    public class Concurrency {
        public void RunOnMainThread(Action action) {
            Scheduler.ExecuteOnMainThread(action);
        }
    }
}