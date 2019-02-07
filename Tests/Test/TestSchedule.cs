using NUnit.Framework;
using Ssc.SscAlgorithm.SscQueue;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Test {
    [TestFixture]
    public class TestSchedule {
        [Test]
        public void TestTimer() {
            var timerQueue = new TimerQueue(10);
            timerQueue.Initialize();

            stack.Push("13000");
            stack.Push("9000");
            stack.Push("4000");
            stack.Push("8000");
            stack.Push("5000");
            stack.Push("2000");

            timerQueue.SetTimer(CallMeBack, "5000", 5000);
            timerQueue.SetTimer(CallMeBack, "2000", 2000);
            timerQueue.SetTimer(CallMeBack, "8000", 8000);

            Thread.Sleep(15000);

            timerQueue.SetTimer(CallMeBack, "13000", 13000);
            timerQueue.SetTimer(CallMeBack, "9000", 9000);
            timerQueue.SetTimer(CallMeBack, "4000", 4000);
        }

        private Stack<string> stack = new Stack<string>();
        private void CallMeBack(Object state) {
            Assert.That(state, Is.EqualTo(stack.Pop()));
        }
    }
}