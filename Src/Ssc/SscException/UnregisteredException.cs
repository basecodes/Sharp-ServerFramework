using System;

namespace Ssc.SscException {
    public class UnregisteredException : Exception {
        public UnregisteredException() {
        }

        public UnregisteredException(string message) : base(message) {
        }

        public UnregisteredException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}