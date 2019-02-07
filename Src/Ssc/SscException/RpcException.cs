using System;

namespace Ssc.SscException {
    //public enum ResponseStatus : byte {
    //    Default = 0,
    //    Success = 1,
    //    Timeout = 2,
    //    Error = 3,
    //    Unauthorized = 4,
    //    Invalid = 5,
    //    Failed = 6,
    //    NotConnected = 7,
    //    NotHandled = 8
    //}

    public class RpcException : Exception {
        public RpcException() {
        }

        public RpcException(string message) : base(message) {
        }

        public RpcException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}