using System;
using System.Threading;
using Ssc.SscAlgorithm.SscQueue;
using Ssc.SscLog;
using UnityEngine;
using Logger = Ssc.SscLog.Logger;

namespace Ssu.Ssu {
    public class SsuLogAppenders : LogAppenders {
        private MessageQueue _messageQueue;

        public SsuLogAppenders() {
            _messageQueue = new MessageQueue();
            _messageQueue.Start();
        }
        public override bool ConsoleAppender(Logger logger, LogLevel logLevel, object message, string callingFilePath,
            string callingMethod, int callingFileLineNumber) {
            if (base.ConsoleAppender(logger, logLevel, message, callingFilePath, callingMethod,
                callingFileLineNumber)) {
                var theadName = Thread.CurrentThread.ManagedThreadId;

                if (logLevel <= LogLevel.Info)
                    _messageQueue.Enqueue(() => {
                        Debug.Log(
                        $"{DateTime.Now.ToString()} [{logLevel} | {logger.Name}] ({callingMethod}:{callingFileLineNumber}) {message}");
                    });

                else if (logLevel <= LogLevel.Warn)
                    _messageQueue.Enqueue(() => {
                        Debug.LogWarning(
                       $"{DateTime.Now.ToString()} [{logLevel} | {logger.Name}] ({callingMethod}:{callingFileLineNumber}) {message}");
                    });
                else if (logLevel <= LogLevel.Fatal)
                    _messageQueue.Enqueue(() => {
                        Debug.LogError(
                        $"{DateTime.Now.ToString()} [{logLevel} | {logger.Name}] ({callingMethod}:{callingFileLineNumber}) {message}");
                    });
                return true;
            }

            return false;
        }
    }
}