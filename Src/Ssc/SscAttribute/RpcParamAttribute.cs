using System;

namespace Ssc.SscAttribute {
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RpcParamAttribute : Attribute {
        public RpcParamAttribute(string paramName = "") {
            ParamName = paramName;
        }

        public string ParamName { get; }
    }
}