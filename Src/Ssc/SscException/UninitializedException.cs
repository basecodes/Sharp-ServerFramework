using System;

namespace Ssc.SscException {    
    public class UninitializedException : Exception {
        public UninitializedException() {
        }

        public UninitializedException(string message) : base(message) {
        }

        public UninitializedException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}