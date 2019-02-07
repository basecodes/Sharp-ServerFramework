using System;
using Ssc.SscRpc;

namespace Ssc.SscAttribute {
     [AttributeUsage(AttributeTargets.Method)]
    public class RpcMethodAttribute:Attribute {
        public string Id { get; }
        public RpcType RpcType { get; }
        public RpcMethodAttribute(string id, Type type = null, RpcType rpcType = RpcType.User) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentNullException(nameof(id));
            }
            Id = id;
            RpcType = rpcType;
        }

        public string GetId() {
            return RpcType + "-" + Id;
        }
    }
}
