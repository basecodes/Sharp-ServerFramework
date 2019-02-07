using System;
using System.Diagnostics;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssc.SscStream;
using Ssc.SscTemplate;

namespace Ssc.SscRpc {
    public struct RpcInvoke : IRpc {
        public string Exception { get; private set; }
        public string MethodId { get; private set; }

        private static readonly Logger Logger = LogManager.GetLogger<RpcInvoke>(LogType.Middle);

        public RpcInvoke(string exception, string methodId) {
            Exception = exception;
            MethodId = methodId;
        }

        public void DeserializeFields(IReadStream readStream) {
            Exception = readStream.ShiftRight<string>();
            MethodId = readStream.ShiftRight<string>();
        }

        public void SerializeFields(IWriteStream writeStream) {
            writeStream.ShiftLeft(MethodId);
            writeStream.ShiftLeft(Exception);
        }

        public void Handle(ulong remoteMessageId, IPeer peer,IReadStream readStream) {
            if (peer == null) {
                Logger.Error($"{nameof(peer)} 为 null");
                return;
            }

            var method = RpcResponseManager.GetRpcMethod(MethodId);
            if (method == null) {
                Logger.Warn($"{MethodId}未注册！");
                return;
            }

            var length = readStream.ShiftRight<byte>();
            Logger.Debug($"MethodId:{MethodId} 参数个数：{length}");

            var deserializable = new Deserializable(readStream);
            var values = deserializable.Deserialize(new object[length + 2], length);

            values[values.Length - 2] = peer;
            Action temp = () => { };
            Action<Action> action = callback => temp += callback;
            values[values.Length - 1] = action;

            ResponseCallback responseCallback = (rm, sd) => {
                temp?.Invoke();
            };

            object result = null;
            var lateBoundMethod = RpcResponseManager.GetRpcMethod(MethodId);
            var exception = "";

            if (lateBoundMethod == null) {
                exception = $"没有方法:{MethodId}";
                Logger.Warn(exception);
            } else {
                var stopwatch = Stopwatch.StartNew();
                try {
                    result = lateBoundMethod.Invoke(values);
                } catch (Exception e) {
                    Logger.Error($"方法：[{MethodId}] " + e);
                    exception = $"方法：[{MethodId}] 发生异常！";
                } finally {
                    stopwatch.Stop();
                }

                Logger.Debug($"当前[{MethodId}]方法执行时间:{stopwatch.ElapsedMilliseconds}毫秒");
            }

            var rpcResponse = new RpcResponse(exception, MethodId,remoteMessageId);
            using (var serializable = new Serializable(null)) {
                serializable.SerializableObject(result);
                RpcProxy.Invoke(MessageType.RpcResponse, rpcResponse, 
                    serializable.WriteStream, peer, responseCallback);
            }
        }
    }
}