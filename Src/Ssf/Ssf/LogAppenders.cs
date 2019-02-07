using System;
using System.IO;
using System.Threading;
using Ssc.SscAlgorithm.SscQueue;
using Ssc.SscConfiguration;
using Ssc.SscExtension;
using Ssc.SscFile;
using Ssc.SscLog;

namespace Ssf.Ssf {
    public class SsfLogAppenders : LogAppenders {

        private MessageQueue _messageQueue;
        private DateTime _dateTime;
        public SsfLogAppenders() {
            _messageQueue = new MessageQueue();
            _messageQueue.Start();
            _dateTime = DateTime.Now;
        }

        private void FileWrite(string log) {
            var now = DateTime.Now;
            var logConfig = LogManager.LogConfig;
            if (_dateTime + TimeSpan.FromSeconds(logConfig.WriteInterval) < now) {
                _dateTime = now;
            }

            var name = Path.GetFileNameWithoutExtension(logConfig.FileName);
            var ext = Path.GetExtension(logConfig.FileName);
            var path = $"{name}-{_dateTime.ToString("yy-mm-dd-hh")}{ext}";
            var directory = Path.GetDirectoryName(logConfig.FileName);
            log += "\n";
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            FileHelper.Write(directory+"/"+path, log.ToBytes2()).Wait();
        }

        public override bool ConsoleAppender(Logger logger, LogLevel logLevel, object message, string callingFilePath,
            string callingMethod, int callingFileLineNumber) {
            if (base.ConsoleAppender(logger, logLevel, message, callingFilePath, callingMethod,
                callingFileLineNumber)) {
                var theadName = Thread.CurrentThread.ManagedThreadId;

                var log = $"{DateTime.Now.ToString()} [{theadName}] [{logLevel} | {logger.Name}] ({callingMethod}:{callingFileLineNumber}) {message}";

                switch (logLevel) {
                    case LogLevel.All:
                        break;
                    case LogLevel.Trace:
                        _messageQueue.Enqueue(() => {
                            if ((LogManager.LogConfig.OutputType & OutputType.File) > 0) {
                                FileWrite(log);
                            }
                            if ((LogManager.LogConfig.OutputType & OutputType.Console) > 0) {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine(log);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        });
                        break;
                    case LogLevel.Debug:
                        _messageQueue.Enqueue(() => {
                            if ((LogManager.LogConfig.OutputType & OutputType.File) > 0) {
                                FileWrite(log);
                            }

                            if ((LogManager.LogConfig.OutputType & OutputType.Console) > 0) {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(log);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        });
                        break;
                    case LogLevel.Info:
                        _messageQueue.Enqueue(() => {
                            if ((LogManager.LogConfig.OutputType & OutputType.File) > 0) {
                                FileWrite(log);
                            }

                            if ((LogManager.LogConfig.OutputType & OutputType.Console) > 0) {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine(log);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        });
                        break;
                    case LogLevel.Warn:
                        _messageQueue.Enqueue(() => {
                            if ((LogManager.LogConfig.OutputType & OutputType.File) > 0) {
                                FileWrite(log);
                            }
                            if ((LogManager.LogConfig.OutputType & OutputType.Console) > 0) {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(log);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        });
                        break;
                    case LogLevel.Error:
                        _messageQueue.Enqueue(() => {
                            if ((LogManager.LogConfig.OutputType & OutputType.File) > 0) {
                                FileWrite(log);
                            }

                            if ((LogManager.LogConfig.OutputType & OutputType.Console) > 0) {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(log);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        });
                        break;
                    case LogLevel.Fatal:
                        _messageQueue.Enqueue(() => {
                            if ((LogManager.LogConfig.OutputType & OutputType.File) > 0) {
                                FileWrite(log);
                            }

                            if ((LogManager.LogConfig.OutputType & OutputType.Console) > 0) {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine(log);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        });
                        break;
                    default:
                        break;
                }

                return true;
            }

            return false;
        }
    }
}