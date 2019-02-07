using System;

namespace Ssc.SscException {

    public class MissingAttributeException : Exception {
        public MissingAttributeException() {
        }

        public MissingAttributeException(string message) : base(message) {
        }

        public MissingAttributeException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
