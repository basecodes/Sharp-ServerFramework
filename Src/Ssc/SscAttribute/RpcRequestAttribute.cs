using System;
using Ssc.SscRpc;

namespace Ssc.SscAttribute {
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcRequestAttribute : RpcMethodAttribute {
        public RpcRequestAttribute(string id,Type type = null,RpcType rpcType = RpcType.User)
            :base(id,type,rpcType){
        }
    }
}