using System;

namespace Ssc.SscException {
    public class NoAttributeException : Exception {
        public NoAttributeException() {
        }

        public NoAttributeException(string message) : base(message) {
        }

        public NoAttributeException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
