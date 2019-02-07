using System;
using Ssc.Ssc;

namespace Ssm.SsmComponent {
    public interface ISecurityComponent :IPeerComponent,IRecyclable,IAssignable,IDisposable {
        string AesKey { get; set; }
        byte[] Encryptkey { get; set; }
    }
}