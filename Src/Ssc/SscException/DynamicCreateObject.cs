using System;

namespace Ssc.SscException {
    public class DynamicCreateObjectException : Exception {
        public DynamicCreateObjectException() {
        }

        public DynamicCreateObjectException(string message) : base(message) {
        }

        public DynamicCreateObjectException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}