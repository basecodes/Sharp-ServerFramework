using System;
using Ssc.SscRpc;

namespace Ssc.SscAttribute {
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcResponseAttribute : RpcMethodAttribute {
        public RpcResponseAttribute(string id,Type type = null,RpcType rpcType = RpcType.User) 
            :base(id,type,rpcType){
        }
    }
}