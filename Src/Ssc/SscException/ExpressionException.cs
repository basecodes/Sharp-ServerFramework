using System;
using System.Collections.Generic;
using System.Text;

namespace Ssc.SscException {
    public class ExpressionException : Exception {
        public ExpressionException() {
        }

        public ExpressionException(string message) : base(message) {
        }

        public ExpressionException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
