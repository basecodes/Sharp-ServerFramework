using System;

namespace Ssf.SsfException {    
    public class NoComponentException : Exception {
        public NoComponentException() {
        }

        public NoComponentException(string message) : base(message) {
        }

        public NoComponentException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}