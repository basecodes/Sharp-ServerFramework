using System;

namespace Ssm.Ssm {    
    public struct Result {
        public object Value { get; set; }
        public Func<Func<object>, object> Function { get; set; }
    }
}