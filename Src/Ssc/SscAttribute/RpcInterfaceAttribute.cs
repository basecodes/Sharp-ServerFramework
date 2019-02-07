using System;

namespace Ssc.SscAttribute {
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcInterfaceAttribute : Attribute {
        public RpcInterfaceAttribute(string guid) {
            if (string.IsNullOrEmpty(guid)) throw new ArgumentNullException(nameof(guid));
            Guid = guid;
        }

        public string Guid { get; }
    }
}