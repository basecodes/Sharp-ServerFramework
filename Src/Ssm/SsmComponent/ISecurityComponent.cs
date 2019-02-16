using System;
using Ssc.Ssc;

namespace Ssm.SsmComponent {
    public interface ISecurityComponent :IPeerComponent,IMemoryable,IDisposable {
        string AesKey { get; set; }
        byte[] Encryptkey { get; set; }
    }
}