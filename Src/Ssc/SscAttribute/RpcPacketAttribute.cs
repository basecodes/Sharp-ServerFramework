using System;

namespace Ssc.SscAttribute {
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
    public class RpcPacketAttribute:Attribute {
        
        public Type BaseType { get; }
        public RpcPacketAttribute(Type baseType) {
            BaseType = baseType;
        }
    }
}