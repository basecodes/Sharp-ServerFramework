using System;

namespace Ssc.SscException {
    public class ArgumentNotNullException : ArgumentException {
        public ArgumentNotNullException() {
        }

        public ArgumentNotNullException(string paramName) : base(paramName) {
        }

        public ArgumentNotNullException(string message, Exception innerException) :
            base(message, innerException) {
        }

        public ArgumentNotNullException(string paramName, string message) :
            base(paramName, message) {
        }
    }

    public class MethodException : Exception {
        private readonly string _methodName;

        public MethodException() {
        }

        public MethodException(string message)
            : base(message) {
        }

        public MethodException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public MethodException(string message, string methodName)
            : this(message) {
            _methodName = methodName;
        }

        public MethodException(string message, string methodName, Exception innerException)
            : this(message, innerException) {
            _methodName = methodName;
        }

        public override string Message {
            get {
                var msg = base.Message;
                if (!string.IsNullOrEmpty(_methodName)) {
                    var resourceString = $"Method Name:{_methodName}";
                    return msg + Environment.NewLine + resourceString;
                }

                return msg;
            }
        }

        public virtual string methodName_ => _methodName;
    }

    public class MethodNotCallException : MethodException {
        public MethodNotCallException() {
        }

        public MethodNotCallException(string message) : base(message) {
        }

        public MethodNotCallException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public MethodNotCallException(string message, string methodName) :
            base(message, methodName) {
        }

        public MethodNotCallException(string message, string methodName,
            Exception innerException) : base(message, methodName, innerException) {
        }
    }
}