using System;

namespace Ssc.SscException {
    public class NetworkException : Exception {
        internal NetworkException(string msg) : base(msg) {
        }

        internal NetworkException(string msg, Exception e) : base(msg, e) {
        }
    }
}